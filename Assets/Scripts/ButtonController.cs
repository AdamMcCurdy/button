using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonController : MonoBehaviour
{
    [Header("Button vertical limits")]
    [SerializeField] private float minY = -0.02f;
    [SerializeField] private float maxY = 0.03f;
    
    [Tooltip("Distance from bottom to activate click event")]
    [SerializeField] private float clickedTolerance = 0.01f;
    
    [Header("Colors for modes")]
    [SerializeField] private Color inactiveColor = Color.red;
    [SerializeField] private Color activeColor = Color.green;
    
    [Tooltip("Time to wait while button is pressed down before returning")]
    [SerializeField] private float waitTime = 1.0f;
    
    [Header("Audio files")]
    [SerializeField] private AudioClip clickedSound;
    [SerializeField] private AudioClip releasedSound;
    
    private AudioSource _audioSource;
    private bool _isClicked;
    private SpringJoint _joint;

    private float _originalSpringTension;
    private float _timeSinceLastClick;

    private void Awake()
    {
        _joint = GetComponent<SpringJoint>();
        _audioSource = GetComponent<AudioSource>();

        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, maxY, pos.z);

        SetButton(1500, inactiveColor, null);
    }
    
    void Update()
    {
        //Track position at all times and keep it explicitly in our boundaries regardless of sprint settings
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, Mathf.Clamp(pos.y, minY, maxY), pos.z);

        //Once we past threshold of min y and click tolerance call the clicked method
        if (transform.localPosition.y < minY + clickedTolerance)
        {
            Clicked();
        }
        
        //Update click timing
        _timeSinceLastClick += Time.deltaTime;
    }

    private void Clicked()
    {
        //If time since click isn't over threshold return
        if ((_timeSinceLastClick < waitTime)) return;
        
        //Reset time since last click
        _timeSinceLastClick = 0f;

        //If this is a fresh click (redundant check)
        if (_isClicked == false)
        {
            //Set flag to protect bouncing
            _isClicked = true;
            
            //Update button state
            SetButton(0, activeColor, clickedSound);
            
            //Set position explicitly for any random spring bugs
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, minY, pos.z);
        }
        else
        {
            //Reset button state
            SetButton(1500, inactiveColor, releasedSound);
            _isClicked = false;
        }
    }

    private void SetButton(float springTension, Color setColor, AudioClip sound)
    {
        _joint.spring = springTension;
        GetComponent<MeshRenderer>().material.color = setColor;
        if (!sound) return;
        _audioSource.clip = sound;
        _audioSource.Play();
    }
}
