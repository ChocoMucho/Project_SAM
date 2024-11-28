using UnityEngine;

public class Player : MonoBehaviour
{
    //=====References=====
    [SerializeField] private GameObject _aimCamera;
    private PlayerInputs _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInputs>();
    }

    void Start()
    {
        _aimCamera.SetActive(false);
        
    }

    void Update()
    {
        if(_input.aim)
        {
            _aimCamera.SetActive(true);
        }
        else
        {
            _aimCamera.SetActive(false);
        }
    }
}
