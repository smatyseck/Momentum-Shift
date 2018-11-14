﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class Player1Movement : MonoBehaviour
{

    public GameObject player2;
    public float jumpForce = 350;
    public float sprintMod = 1.5f;
    public GameObject readytext;
    public GameObject pausemenu;

    // What is the maximum speed we want Bob to walk at
    public float maxSpeed = 5f;

    // Start facing right (like the sprite-sheet)
    private bool facingLeft = false;

    // This will be a reference to the RigidBody2D 
    // component in the Player object
    private Rigidbody2D rb;

    // This is a reference to the Animator component
    private Animator anim;
    private bool grounded = false;
    private bool ready = false;
    
	//Checkpoint Tracking
	public GameObject checkpoint;

	public bool IsReady()
	{
		return ready;
	}

	public void SetReady(bool r)
	{
		ready = r;
	}

	void Start()
	{
		rb = gameObject.GetComponent<Rigidbody2D>();
		anim = gameObject.GetComponent<Animator>();
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Checkpoint")
        {
            checkpoint = col.gameObject;
        }
        if (col.tag == "Zapper")
        {
            this.transform.position = checkpoint.transform.position;
            rb.velocity = new Vector2(0, 0);
        }
        if (col.tag == "Ground")
        {
            grounded = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Ground")
        {
            grounded = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Ground")
        {
            grounded = false;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }
        if (Input.GetButtonDown("Cancel"))
        {
            Time.timeScale = 0f;
            pausemenu.SetActive(true);
        }

        if (Input.GetButtonDown("Reset"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetButtonDown("LastCheckpoint"))
        {
            rb.position = checkpoint.transform.position;
        }

        if (Input.GetButtonDown("Swap"))
        {
            if (player2.GetComponent<Player2Movement>().IsReady())
            {
                ready = false;
                player2.GetComponent<Player2Movement>().SetReady(false);
                swap();
                readytext.SetActive(false);
            }
            else {
                ready = true;
                Text text = readytext.GetComponent<Text>();
                text.text = "Player 1 is Ready to Swap\n3";
                readytext.SetActive(true);
            }   
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
        }

        if (grounded)
        {
            // Get the extent to which the player is currently pressing left or right
            float h = Input.GetAxis("Horizontal");
            if (Math.Abs(h) < 0.1f)
            {
                h = 0;
            }
            float newspeed = 0;
            if ((rb.velocity.x > 0 && h < 0) || (rb.velocity.x < 0 && h > 0))
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            float limitSpeed = maxSpeed;
            if (Input.GetButton("Sprint"))
            {
                limitSpeed = maxSpeed * sprintMod;
                newspeed = rb.velocity.x + (h * limitSpeed) / 5;
            } else
            {
                newspeed = rb.velocity.x + (h * limitSpeed) / 5;
            }
            if (newspeed > limitSpeed)
            {
                newspeed = limitSpeed;
            }
            else if (newspeed < -1 * limitSpeed)
            {
                newspeed = -1 * limitSpeed;
            }
            if (h == 0)
            {
                //Slow down quicker than accelerate
                newspeed = 0f;
            }
            rb.velocity = new Vector2(newspeed, rb.velocity.y);
            // Check which way the player is facing 
            // and call reverseImage if neccessary
            if (h < 0 && !facingLeft)
                reverseImage();
            else if (h > 0 && facingLeft)
                reverseImage();
        }

        anim.SetBool("grounded", grounded);
        anim.SetFloat("speedY", rb.velocity.y);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));

    }

    void reverseImage()
    {
        // Switch the value of the Boolean
        facingLeft = !facingLeft;

        // Get and store the local scale of the RigidBody2D
        Vector2 theScale = rb.transform.localScale;

        // Flip it around the other way
        theScale.x *= -1;
        rb.transform.localScale = theScale;
    }

    void swap()
    {
        Vector2 pos1 = rb.position;
        Vector2 pos2 = player2.GetComponent<Rigidbody2D>().position;

        rb.position = pos2;
        //Swap Checkpoints as well
        player2.GetComponent<Rigidbody2D>().position = pos1;
        GameObject c = player2.GetComponent<Player2Movement>().checkpoint;
        player2.GetComponent<Player2Movement>().checkpoint = checkpoint;
        checkpoint = c;
    }

}
 