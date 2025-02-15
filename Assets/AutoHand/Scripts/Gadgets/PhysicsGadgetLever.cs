using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Autohand{

    [System.Serializable]
    public struct StepEvent {
        public int step;
        public UnityEvent OnStepEnter;
        public UnityEvent OnStepExit;
    }

    public class PhysicsGadgetLever : PhysicsGadgetHingeAngleReader {
        [Min(0.01f), Tooltip("The percentage (0-1) from the required value needed to call the event, if threshold is 0.1 OnMax will be called at 0.9, OnMin at -0.9, and OnMiddle at -0.1 or 0.1")]
        public float threshold = 0.05f;

        [Min(0)]
        public int stepCount = 0;
        public int startStep = 0;
        private int prevStepCount = -1;

        public UnityEvent OnMax;
        public UnityEvent OnMid;
        public UnityEvent OnMin;
        public StepEvent[] stepEvents;

        public bool useLocalAngles = false;
        public Vector3 axis;

        public float minAngle = 0f;
        public float maxAngle = 40f;
        public bool reverseValue = false;
        
        bool min = false;
        bool max = false;
        bool mid = true;

        private int currStep = -1;
        private int prevStep = -1;
    
        private float minimum;
        private float maximum;
        float[] stepMarkers;

		public float GetNormalizedLocalRotation(Transform obj, Vector3 localAxis, float minAngle, float maxAngle)
		{
			// Obtener la rotación local en torno al eje deseado
			float angle = Quaternion.Angle(Quaternion.identity, obj.localRotation);

			// Obtener el signo correcto del ángulo
			Vector3 rotatedVector = obj.localRotation * Vector3.forward;
			float signedAngle = Vector3.SignedAngle(Vector3.forward, rotatedVector, localAxis);

			// Clampeamos el ángulo a los valores min y max
			signedAngle = Mathf.Clamp(signedAngle, minAngle, maxAngle);

            // Normalizamos el valor entre 0 y 1
            float normalizedValue = Mathf.InverseLerp(minAngle, maxAngle, signedAngle);
            Debug.Log("PRE :" + normalizedValue);
            normalizedValue = normalizedValue *2f -1f;

            if (reverseValue)
                return normalizedValue = -normalizedValue;
            return normalizedValue;
		}

		protected void FixedUpdate(){
            float value = 0;

			if (useLocalAngles)
                value = GetNormalizedLocalRotation(this.transform, axis, minAngle, maxAngle);
            else
                value = GetValue();

            Debug.Log("VALUE :"+value);
            if(!max && mid && value+threshold >= 1) {
                Max();
            }

            if(!min && mid && value-threshold <= -1){
                Min();
            }
        
            if (value <= threshold && max && !mid) {
                Mid();
            }

            if (value >= -threshold && min && !mid) {
                Mid();
            }
        }

        protected override void Start() {
            base.Start();
            if(startStep <= 0) return;

            FindSteps();
            SetSpring(startStep - 1);
        }

        void Update() {
            if(!useLocalAngles)
                AdjustStep();
        }

        void AdjustStep() {
            if(stepCount <= 0) return;

            FindSteps();
            SetSpring(FindCurrentStep()); 
        }

        bool FindSteps() {
            if(prevStepCount == stepCount) return false;

            prevStepCount = stepCount;

            stepMarkers = new float[stepCount];

            minimum = GetJoint().limits.min;
            maximum = GetJoint().limits.max;
        
            float step = GetStep();

            for(int i = 0; i < stepCount; i++) {
                stepMarkers[stepCount - i - 1] = minimum + (i * step);
            }

            return true;
        }

        public void SetSpring(int step)
        {
            GetJoint().transform.localRotation *= Quaternion.Euler(GetJoint().axis  * stepMarkers[step]);

            currStep = step;
            JointSpring jointSpring = GetJoint().spring;
            jointSpring.targetPosition = stepMarkers[step]; 
            GetJoint().spring = jointSpring;
        }

        public void SetSpring(float stepRotation)
        {
            JointSpring jointSpring = GetJoint().spring;
            jointSpring.targetPosition = stepRotation; 
            GetJoint().spring = jointSpring;
        }

        float FindCurrentStep() {

			float value = 0;

			if (useLocalAngles)
				value = GetNormalizedLocalRotation(this.transform, axis, minAngle, maxAngle);
			else
				value = GetValue();

			float checkValue = value * GetRange();
            for(int i = 0; i < stepCount; i++) {
                if(checkValue >= GetMinimumStep(i) && checkValue <= GetMaximumStep(i)) {
                    currStep = i;
                    if(currStep != prevStep) {
                        Step();
                        prevStep = currStep;
                    }

                    return stepMarkers[i];
                }
            }

            return 0;
        }

        float GetStep() => (Mathf.Abs(minimum) + Mathf.Abs(maximum)) / (stepCount - 1);
        float GetRange() => (Mathf.Abs(minimum) + Mathf.Abs(maximum)) / 2;
        float GetMinimumStep(int index) => stepMarkers[index] - (GetStep() / 2);
        float GetMaximumStep(int index) => stepMarkers[index] + (GetStep() / 2);

        void Max() {
            mid = false;
            max = true;
            OnMax?.Invoke();
        }

        void Mid() {
            min = false;
            max = false;
            mid = true;
            OnMid?.Invoke();
        }

        void Min() {
            min = true;
            mid = false;
            OnMin?.Invoke();
        }

        void Step() {
            for(int i = 0; i < stepEvents.Length; i++) {
                if(stepEvents[i].step == currStep + 1) {
                    stepEvents[i].OnStepEnter?.Invoke();
                }
                else if(stepEvents[i].step == prevStep + 1) {
                    stepEvents[i].OnStepExit?.Invoke();
                }
            }
        }
    }
}
