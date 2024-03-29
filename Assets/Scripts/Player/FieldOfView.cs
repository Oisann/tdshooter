﻿using UnityEngine;
using Oisann.Networking;
using System.Collections.Generic;

namespace Oisann.Player {
	public class FieldOfView : MonoBehaviour {

		public float viewRadius;
		[Range(0, 360)]
		public float viewAngle;

		public float meshResolution;
		public int edgeResolveIterations;
		public float edgeDistanceThreshold;

		public LayerMask obstacleMask;
		public MeshFilter viewMeshFilter;

		private Mesh viewMesh;

		private void Start() {
			viewMesh = new Mesh();
			viewMesh.name = "View Mesh";
			viewMeshFilter.mesh = viewMesh;
		}

		private void LateUpdate() {
			DrawFieldOfView();
		}

		private void DrawFieldOfView() {
			int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
			float stepAngleSize = viewAngle / stepCount;
			List<Vector3> viewPoints = new List<Vector3>();
			ViewCastInfo oldViewCast = new ViewCastInfo();
			for(int i = 0; i <= stepCount; i++) {
				float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
				ViewCastInfo newViewCast = ViewCast(angle);

				if(i > 0) {
					bool edgeDistanceThresholdExceeded = ExceededEdgeDistanceThreshold(oldViewCast, newViewCast);
					if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded)) {
						EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
						if(edge.pointA != Vector3.zero) {
							viewPoints.Add(edge.pointA);
						}
						if(edge.pointB != Vector3.zero) {
							viewPoints.Add(edge.pointB);
						}
					}
				}

				viewPoints.Add(newViewCast.point);
				oldViewCast = newViewCast;
			}

			int vertexCount = viewPoints.Count + 1;
			Vector3[] vertices = new Vector3[vertexCount];
			int[] triangles = new int[(vertexCount - 2) * 3];
			vertices[0] = Vector3.zero;
			for(int i = 0; i < vertexCount - 1; i++) {
				vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);

				if(i < vertexCount - 2) {
					triangles[i * 3] = 0;
					triangles[i * 3 + 1] = i + 1;
					triangles[i * 3 + 2] = i + 2;
				}
				
			}

			viewMesh.Clear();
			viewMesh.vertices = vertices;
			viewMesh.triangles = triangles;
			viewMesh.RecalculateNormals();
		}

		private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
			float minAngle = minViewCast.angle;
			float maxAngle = maxViewCast.angle;
			Vector3 minPoint = Vector3.zero;
			Vector3 maxPoint = Vector3.zero;
			for(int i = 0; i < edgeResolveIterations; i++) {
				float angle = (minAngle + maxAngle) / 2;
				ViewCastInfo newViewCast = ViewCast(angle);

				bool edgeDistanceThresholdExceeded = ExceededEdgeDistanceThreshold(minViewCast, newViewCast);
				if(newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded) {
					minAngle = angle;
					minPoint = newViewCast.point;
				} else {
					maxAngle = angle;
					maxPoint = newViewCast.point;
				}
			}
			return new EdgeInfo(minPoint, maxPoint);
		}

		private bool ExceededEdgeDistanceThreshold(ViewCastInfo oldViewCast, ViewCastInfo newViewCast) {
			return Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
		}

		private ViewCastInfo ViewCast(float globalAngle) {
			Vector3 dir = DirFromAngle(globalAngle, true);
			RaycastHit hit;
			if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask)) {
				return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
			}
			return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}

		public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
			if(!angleIsGlobal) {
				angleInDegrees += transform.eulerAngles.y;
			}
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}

		public struct ViewCastInfo {
			public bool hit;
			public Vector3 point;
			public float distance;
			public float angle;

			public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle) {
				hit = _hit;
				point = _point;
				distance = _distance;
				angle = _angle;
			}
		}

		public struct EdgeInfo {
			public Vector3 pointA;
			public Vector3 pointB;

			public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
				pointA = _pointA;
				pointB = _pointB;
			}
		}
	}
}