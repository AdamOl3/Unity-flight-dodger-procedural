using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gracz : MonoBehaviour
{
    public float grawitacja = 20f;
    public float wysokoscSkoku = 2.8f;
    public float predkoscRuchu = 4f;
    

    Rigidbody rigidbodyGracza;
    bool naZiemi = false;
    Vector3 domyslnaSkala;
    bool kuca = false;

    void Start()
    {
        rigidbodyGracza = GetComponent<Rigidbody>();
        rigidbodyGracza.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbodyGracza.useGravity = false; 
        domyslnaSkala = transform.localScale;
    }

    void Update()
    {
        float ruchPoziomy = Input.GetAxis("Horizontal");   
        float ruchPionowy = Input.GetAxis("Vertical");     

        float ruchGoraDol = 0f;                            
        if (Input.GetKey(KeyCode.Space)) ruchGoraDol = 1f; 
        if (Input.GetKey(KeyCode.LeftControl)) ruchGoraDol = -1f; 

        Vector3 predkosc = rigidbodyGracza.velocity;
        predkosc.x = ruchPoziomy * predkoscRuchu;  
        predkosc.z = ruchPionowy * predkoscRuchu;  
        predkosc.y = ruchGoraDol * predkoscRuchu;  
     
        rigidbodyGracza.velocity = predkosc;
    }

    void FixedUpdate()
    {
        if (!Input.GetKey(KeyCode.Space))
        {
            rigidbodyGracza.AddForce(new Vector3(0, -grawitacja * rigidbodyGracza.mass, 0));
        }

        naZiemi = false;
    }

    void OnCollisionStay()
    {
        naZiemi = true;
    }

    float ObliczPredkoscSkoku()
    {
        return Mathf.Sqrt(2 * wysokoscSkoku * grawitacja);
    }

    void OnCollisionEnter(Collision kolizja)
    {
        if (kolizja.gameObject.tag == "Finish")
        {
            GeneratorPodloza.instancja.graSkonczona = true;
        }
    }
}

