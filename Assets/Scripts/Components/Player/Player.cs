using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private Rigidbody2D rb;

    public float move_speed;
    private Vector2 move_input;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        AwakeAnimation();
    }

    private void Update() {
        
        UpdateMovement();
        UpdateAnimation();
    }

    private void FixedUpdate() {
        Move();
    }

    private void UpdateMovement() {
        var move_input_x = Input.GetAxisRaw("Horizontal");
        var move_input_y = Input.GetAxisRaw("Vertical");
        var is_shift_down = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        move_input = new Vector2(move_input_x, move_input_y).normalized;
        if (is_shift_down) move_input *= 0.5f;

        isMoving = move_input != Vector2.zero;
    }

    private void Move() {
        rb.MovePosition(rb.position + move_speed * Time.deltaTime * move_input);
    }

    // MARK: Animation

    private Animator[] animators;
    private bool isMoving;

    private void AwakeAnimation() {
        animators = GetComponentsInChildren<Animator>();
    }

    private void UpdateAnimation() {
        foreach (var animator in animators) {
            animator.SetBool("isMoving", isMoving);
            if(isMoving && Input.GetKeyDown(KeyCode.T)) animator.SetBool("isSkillPlaying", true);
            if (isMoving) {
                // 使用isMoving是为了让角色停止时，仍然会面朝移动方向
                animator.SetFloat("inputX", move_input.x);
                animator.SetFloat("inputY", move_input.y);
            }
        }
    }
}