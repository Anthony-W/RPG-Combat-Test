using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    protected Animator animator;
    protected float health = 100;

    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    protected void Move(float strafe, float forward)
    {
        animator.SetFloat("forward", forward);
        animator.SetFloat("strafe", strafe);

        transform.Translate(strafe * Time.deltaTime * 3.0f, 0, forward * Time.deltaTime * 3.0f);
    }

    protected void RotateCharacter(float yaw)
    {
        Vector3 newVector = this.transform.eulerAngles;
        newVector.y += yaw;
        this.transform.eulerAngles = newVector;
    }

    public void TakeDamage(float damage, Character attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
            if (attacker is Player) (attacker as Player).ClearTarget();
        }
    }

    protected void Die()
    {
        Destroy(this.gameObject);
    }

    public float DistanceTo(Character c)
    {
        Vector3 vectorDistance = this.transform.position - c.transform.position;
        float linearDistance = vectorDistance.magnitude;
        return Mathf.Abs(linearDistance);
    }
}
