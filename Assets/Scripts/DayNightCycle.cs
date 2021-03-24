using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;
    public UnityEvent<PartOfDay> OnDayPeriodChange = new UnityEvent<PartOfDay>();

    [SerializeField]
    private AnimationCurve _IntensityCurve;
    [SerializeField, Range(0, 24)]
    public float _TimeValue;
    public float _TimeProgress;
    [SerializeField]
    private Light2D _GlobalLight;
    [SerializeField, Range(0, .5f)]
    private float _MinIntensity;
    [SerializeField, Range(.5f, 1f)]
    private float _MaxInstensity;
    [SerializeField, Header("Day length in seconds")]
    private float DayLength;

    public PartOfDay PeriodOfDay
    {
        get
        {
            if (_TimeValue > 2 && _TimeValue <= 9)
                return PartOfDay.Morning;
            if (_TimeValue > 9 && _TimeValue <= 13)
                return PartOfDay.Noon;
            if (_TimeValue > 13 && _TimeValue <= 17)
                return PartOfDay.Afternoon;
            if (_TimeValue > 17 && _TimeValue <= 21)
                return PartOfDay.Evening;
            if (_TimeValue > 21 || _TimeValue <= 2)
                return PartOfDay.Night;
            return PartOfDay.Night;
        }
    }


    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Application.isPlaying)
        {
            PartOfDay _lastTimePeriod = PeriodOfDay;
            _TimeProgress += Time.deltaTime;
            _TimeProgress %= DayLength;
            _TimeValue = 24 * _TimeProgress / DayLength;
            if (_lastTimePeriod != PeriodOfDay)
                OnDayPeriodChange.Invoke(PeriodOfDay);
        }
        _GlobalLight.intensity = Mathf.Clamp(_IntensityCurve.Evaluate(_TimeValue / 24f), _MinIntensity, _MaxInstensity);
    }
}

public enum PartOfDay
{
    Morning,
    Noon,
    Afternoon,
    Evening,
    Night
}