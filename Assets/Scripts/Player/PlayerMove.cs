using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _currentSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentSpeed = _moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
       float move =  Input.GetAxis("Horizontal") * _moveSpeed;
       transform.Translate(move * Time.deltaTime, 0, 0);


        switch (move) 
        {
            case > 0:
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case < 0:
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }

        if (_currentSpeed < 0.1f)
       {
            _currentSpeed = 0;
       }
    }
}
