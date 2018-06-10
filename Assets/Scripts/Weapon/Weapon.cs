using UnityEngine;

namespace Oisann.Weapon {
    public class Weapon : MonoBehaviour {
        public Settings settings;
        public int rounds = 0;
        public float inaccuracyTimer = 0f;

        private float lowAmmoPercentage = 10f;
        private float lastShotTime = float.MinValue;

        private void Awake() {
            //rounds = Mathf.Min(rounds, settings.maxRounds);
            inaccuracyTimer = Mathf.Min(inaccuracyTimer, settings.inaccuracyMaxTime);
        }

        private void Update() {
            if(Input.GetKey(KeyCode.Mouse0)) {
                float rpmHerz = settings.RPM2Hz(settings.CalculateRPM(inaccuracyTimer));
                float nextShot = lastShotTime + rpmHerz;

                if(Time.time >= nextShot) {
                    if(rounds > 0) {
                        lastShotTime = Time.time;
                        if(rounds > Mathf.CeilToInt((settings.maxRounds / 100) * lowAmmoPercentage)) {
                            //Play low ammo sound
                        }
                        float inaccuracy = Random.Range(0, settings.inaccuracyMaxAngle / 2f) * settings.inaccuracy.Evaluate(inaccuracyTimer);
                        //50% chance of picking a negative angle, aka bullet goes left 50% of the time and right the other 50%.
                        inaccuracy = Random.Range(0, 2) == 1 ? inaccuracy * -1f : inaccuracy;
                        Vector3 bulletDirection = new Vector3(0, inaccuracy, 0);

                        rounds = Mathf.Max(0, rounds - 1);
                        GameObject bullet = new GameObject("Bullet");
                        bullet.transform.position = transform.position;
                        bullet.transform.rotation = Quaternion.Euler(bulletDirection + transform.eulerAngles);
                        bullet.AddComponent<Bullet>();
                    } else {
                        //Play no ammo sound
                    }
                }

                inaccuracyTimer = Mathf.Min(settings.inaccuracyMaxTime, inaccuracyTimer + Time.deltaTime);
            } else {
                inaccuracyTimer = Mathf.Max(0.00f, inaccuracyTimer - (Time.deltaTime * 1.5f));
            }
        }
    }
}