using UnityEngine;
using UnityEngine.UI;


public class MyPlayerUI : MonoBehaviour
{
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
    
    private MyPlayerManager _target;
    private Transform _targetTransform;
    private Vector3 _targetPosition;
    private float _characterControllerHeight;

    private void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    
    private void Update()
    {
        if (_target == null) {
            Destroy(gameObject);
            return;
        }
        
        if (playerHealthSlider != null) {
            playerHealthSlider.value = _target.Health;
        }
    }

    private void LateUpdate()
    {
        if (_targetTransform!=null)
        {
            _targetPosition = _targetTransform.position;
            _targetPosition.y += _characterControllerHeight;
				
            transform.position = Camera.main.WorldToScreenPoint (_targetPosition) + screenOffset;
        }
    }

    public void SetTarget(MyPlayerManager target){

        if (target == null) {
            Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency because we are going to reuse them.
        _target = target;
        _targetTransform = _target.GetComponent<Transform>();


        CharacterController _characterController = _target.GetComponent<CharacterController> ();

        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null){
            _characterControllerHeight = _characterController.height;
        }

        if (playerNameText != null) {
            playerNameText.text = _target.photonView.Owner.NickName;
        }
    }
}
