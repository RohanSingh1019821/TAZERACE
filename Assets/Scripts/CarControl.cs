using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public List<GameObject> springs;
    private Rigidbody rb;
    public GameObject propulsion;
    public GameObject centerMass;
    public Transform turnTarget;
    public Transform mesh;
    public GameObject cam;
    public GameObject animator; 

    public GameObject tazeEffect;
    public GameObject blazeEffect;
    public GameObject zoomEffect;
    public GameObject saveEffect;
    public GameObject loadEffect;

    public AudioSource zap;
    public AudioSource zapA;
    public AudioSource zapB;
    public AudioSource zapC;
    public AudioSource zapD;


    public float fThrustBase = 500;
    public float sThrust = 1f;
    public float rThrust = 500;
    public float springThrust = 100f;
    public float springDist = 4f;
    public float boostMult = 2f;
    public float flipDist = 2f;
    public float flipThrust = 10000;
    public float flipTime = 1f;
    public float tazeTime = 3f;

    private float shakeMagnitude = 1f;
    private float tazeTimer = 0f;
    private bool flipping = false;
    private float flipTimer = 0f;

    private Quaternion checkpointRot;
    private Vector3 checkpointPos;

    private Vector3 vel;

    public bool tazed = false;
    public bool blazed = false;
    public bool zoomed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerMass.transform.localPosition;
        checkpointRot = this.transform.rotation;
        checkpointPos = this.transform.position;
        saveEffect.gameObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetButtonDown("Save"))
        {
            saveEffect.gameObject.SetActive(false);
            checkpointRot = this.transform.rotation;
            checkpointPos = this.transform.position;
            saveEffect.gameObject.SetActive(true);
        }

        else if (Input.GetButtonDown("Load"))
        {
            loadEffect.gameObject.SetActive(false);
            this.transform.rotation = checkpointRot;
            this.transform.position = checkpointPos;
            rb.velocity = new Vector3();
            loadEffect.gameObject.SetActive(true);
        }

        if (tazed)
        {
            TazeShake();
            tazeTimer -= Time.deltaTime;
        }
        else if(blazed)
        {
            TazeShake();
            tazeTimer -= Time.deltaTime;
            float fThrust = fThrustBase;
            
            fThrust = fThrustBase;


            rb.AddForceAtPosition(Time.deltaTime * transform.TransformDirection(Vector3.forward) * Input.GetAxis("IVertical") * fThrust, propulsion.transform.position);
            rb.AddTorque(Time.deltaTime * transform.TransformDirection(Vector3.forward) * -Input.GetAxis("IHorizontal") * rThrust);

            Vector3 targetDelta = turnTarget.position - transform.position;
            float angle = Vector3.Angle(transform.forward, targetDelta);
            Vector3 cross = Vector3.Cross(transform.forward, targetDelta);
            rb.AddTorque(cross * angle * sThrust);

            foreach (GameObject spring in springs)
            {
                RaycastHit hit;
                if (Physics.Raycast(spring.transform.position, transform.TransformDirection(Vector3.down), out hit, springDist, 1 << 9)) ;
                {
                    if (hit.distance > 0 && hit.collider.gameObject.layer == 9)
                    {
                        rb.AddForceAtPosition(Time.deltaTime * transform.TransformDirection(Vector3.up) * Mathf.Pow(springDist - hit.distance, 2) / springDist * springThrust, spring.transform.position);
                    }
                }
            }

            rb.AddForce(-Time.deltaTime * transform.TransformVector(Vector3.right) * transform.InverseTransformVector(rb.velocity).x * 10f);

            vel = rb.velocity;
        }
        else
        {
            float fThrust = fThrustBase;
            
            if (zoomed)
            {
                fThrust = fThrustBase * boostMult * 2;
                tazeTimer -= Time.deltaTime;
                TazeShake();
            }
            else
            {
                fThrust = fThrustBase;
            }


            rb.AddForceAtPosition(Time.deltaTime * transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical") * fThrust, propulsion.transform.position);
            rb.AddTorque(Time.deltaTime * transform.TransformDirection(Vector3.forward) * -Input.GetAxis("Horizontal") * rThrust);

            Vector3 targetDelta = turnTarget.position - transform.position;
            float angle = Vector3.Angle(transform.forward, targetDelta);
            Vector3 cross = Vector3.Cross(transform.forward, targetDelta);
            rb.AddTorque(cross * angle * sThrust);
            
            foreach (GameObject spring in springs)
            {
                RaycastHit hit;
                if (Physics.Raycast(spring.transform.position, transform.TransformDirection(Vector3.down), out hit, springDist, 1 << 9)) ;
                {
                    if (hit.distance > 0)
                    {
                        rb.AddForceAtPosition(Time.deltaTime * transform.TransformDirection(Vector3.up) * Mathf.Pow(springDist - hit.distance, 2) / springDist * springThrust, spring.transform.position);
                    }
                }
            }

            rb.AddForce(-Time.deltaTime * transform.TransformVector(Vector3.right) * transform.InverseTransformVector(rb.velocity).x * 10f);

            vel = rb.velocity;
        }
        if(tazeTimer <= 0)
        {
            tazed = false;
            blazed = false;
            zoomed = false;
            tazeEffect.gameObject.SetActive(false);
            blazeEffect.gameObject.SetActive(false);
            zoomEffect.gameObject.SetActive(false);
            cam.GetComponent<LookControl>().UnBlaze();
            mesh.localPosition = new Vector3(0, 0, 0);
            animator.gameObject.SetActive(false);
            animator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }

        FlipCheck();
    }

    void FlipCheck()
    {
        if (flipping)
        {
            rb.AddTorque(transform.TransformDirection(Vector3.forward) * flipThrust);
            flipTimer -= Time.deltaTime;
        }

        if (flipTimer <= 0 )
        {
            flipping = false;
        }

        RaycastHit hitt;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hitt, flipDist, 1 << 9))
        {
            if (!flipping && hitt.collider.gameObject.layer == 9);
            {
                flipping = true;
                print("flip");
                flipTimer = flipTime;
            }
        }
    }

    void TazeShake()
    {
        mesh.localPosition = new Vector3(0, 0, 0);
        float x = Random.Range(-0.1f, 0.1f) * shakeMagnitude;
        float y = Random.Range(-0.1f, 0.1f) * shakeMagnitude;
        float z = Random.Range(-0.1f, 0.1f) * shakeMagnitude;
        mesh.localPosition += new Vector3(x, y, z);



        animator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        float xUI = Random.Range(-100.0f, 0f) * shakeMagnitude;
        float yUI = Random.Range(-100.0f, 0f) * shakeMagnitude;
        animator.GetComponent<RectTransform>().anchoredPosition += new Vector2(xUI, yUI);
    }

    public void GetTazed()
    {
        print("TAZED");
        if (!tazed)
        {
            Zap();
        }
        tazed = true;
        tazeTimer = tazeTime;
        tazeEffect.gameObject.SetActive(true);
        animator.GetComponent<SpriteRenderer>().color = Color.cyan;

        animator.gameObject.SetActive(true);
        animator.GetComponent<Animator>().Play("Tazed", -1, Random.Range(0f, 1f));

    }
    
    public void GetBlazed()
    {
        print("BLAZED");
        if (!blazed)
        {
            Zap();
        }
        blazed = true;
        tazeTimer = tazeTime;
        blazeEffect.gameObject.SetActive(true);
        cam.GetComponent<LookControl>().Blaze();
        animator.GetComponent<SpriteRenderer>().color = Color.red;

        animator.gameObject.SetActive(true);
        animator.GetComponent<Animator>().Play("Tazed", -1, Random.Range(0f, 1f));

    }

    public void GetZoomed()
    {
        print("ZOOMED");
        if (!zoomed)
        {
            Zap();
        }
        zoomed = true;
        tazeTimer = tazeTime;
        zoomEffect.gameObject.SetActive(true);
        animator.GetComponent<SpriteRenderer>().color = Color.green;

        animator.gameObject.SetActive(true);
        animator.GetComponent<Animator>().Play("Tazed", -1, Random.Range(0f, 1f));

    }

    public void Zap()
    {
        zap.Play();
        int clipID = Random.Range(0, 3);
        if (clipID == 0)
        {
            zapA.Play();
        }
        if (clipID == 1)
        {
            zapB.Play();
        }
        if (clipID == 2)
        {
            zapC.Play();
        }
        if (clipID == 3)
        {
            zapD.Play();
        }
    }
}
