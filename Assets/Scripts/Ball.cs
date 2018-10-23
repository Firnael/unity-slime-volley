using System;
using UnityEngine;

public class Ball : MonoBehaviour {

    public GameObject ballPieces;
    public float maximumVerticalVelocity;
    public float maximumHorizontalVelocity;
    public float fireParticleSpawnScaling;
    public float fireParticleSpawnCondition;

    private Rigidbody2D body;

	void Start () {
        body = GetComponent<Rigidbody2D>();
        Init();
    }

    public void Init()
    {
        gameObject.SetActive(true);
        Physics2D.IgnoreCollision(body.gameObject.GetComponent<CircleCollider2D>(), GameObject.FindGameObjectWithTag("SuperNet").GetComponent<BoxCollider2D>());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            if(body.position.x < 0)
            {
                GameManager.instance.AddPoint(true);
            } 
            else
            {
                GameManager.instance.AddPoint(false);
            }
            Explode();
        }

        if (collision.gameObject.tag == "Player")
        {
            body.velocity = body.velocity * 1.5f;
        }
    }

    void Update () {
        // Particles
        ParticleSystem.EmissionModule emissionModule = GetComponent<ParticleSystem>().emission;
        float fireParticleSpawnRate = body.velocity.magnitude > fireParticleSpawnCondition ? body.velocity.magnitude / fireParticleSpawnScaling : 0;
        emissionModule.rateOverDistanceMultiplier = fireParticleSpawnRate;

        // Ball color
        GetComponent<SpriteRenderer>().color = body.velocity.magnitude > fireParticleSpawnCondition ? Color.red : Color.white;

        // Limit max velocity
		if(Math.Abs(body.velocity.y) > maximumVerticalVelocity)
        {
            body.velocity = new Vector2(body.velocity.x, maximumVerticalVelocity * Math.Sign(body.velocity.y));
        }
        if (Math.Abs(body.velocity.x) > maximumHorizontalVelocity)
        {
            body.velocity = new Vector2(maximumHorizontalVelocity * Math.Sign(body.velocity.x), body.velocity.y);
        }
    }

    void Explode()
    {
        GameObject pieces = Instantiate(ballPieces, body.transform.position, body.transform.rotation);
        foreach (Rigidbody2D pieceBody in pieces.GetComponentsInChildren<Rigidbody2D>())
        {
            Physics2D.IgnoreCollision(pieceBody.GetComponent<PolygonCollider2D>(), GameObject.FindGameObjectWithTag("SuperNet").GetComponent<BoxCollider2D>());
            pieceBody.velocity = new Vector2(body.velocity.x, 0);
            pieceBody.rotation = body.rotation;
        }

        gameObject.SetActive(false);
    }
}
