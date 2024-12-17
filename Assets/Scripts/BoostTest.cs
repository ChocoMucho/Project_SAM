using UnityEngine;

public class BoostTest : MonoBehaviour
{
    public float InitialSpeed;
    public float TargetSpeed;
    public float CurrentSpeed;
    public float Duration;
    public float ElapsedTime;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        ElapsedTime = 0f;
        transform.position = new Vector3(0f, 0f, 1f);
    }

    void Update()
    {
        if(ElapsedTime >= Duration)
        {
            ElapsedTime = 0f;
            transform.position = new Vector3 (0f, 0f, 1f);
        }
        else
        {
            ElapsedTime += Time.deltaTime;
            Move();
        }
        
    }

    void Move()
    {        
        CurrentSpeed = Mathf.Lerp(InitialSpeed, TargetSpeed, ElapsedTime / Duration);
        characterController.Move(Vector3.forward * CurrentSpeed * Time.deltaTime);
    }
}
