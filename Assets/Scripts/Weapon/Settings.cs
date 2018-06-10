using UnityEngine;

namespace Oisann.Weapon {
    [CreateAssetMenu(fileName = "Weapon", menuName = "Inventory/Weapon")]
    public class Settings : ScriptableObject {
        public string weaponName = "";
        public int maxRounds = 100;
        public float reloadTime = 5.0f;

        [Header("Rounds per minute")]
        public int rpm = 90;
        public float rpmMaxTime = 1.0f;
        public AnimationCurve effectiveRPM = new AnimationCurve();
        
        [Header("Inaccuracy")]
        public float inaccuracyMaxTime = 1.0f;
        [Range(0, 359)]
        public float inaccuracyMaxAngle = 90.0f;
        public AnimationCurve inaccuracy = new AnimationCurve();

        public int CalculateRPM(float time) {
            return Mathf.RoundToInt(this.effectiveRPM.Evaluate(Mathf.Min(time, rpmMaxTime)) * this.rpm);
        }

        public float RPM2Hz(int RPM) {
            if(RPM == 0)
                return 0;
            float m = (RPM / 60);
            if(m == 0)
                return 1;
            return 1 / m;
        }
    }
}