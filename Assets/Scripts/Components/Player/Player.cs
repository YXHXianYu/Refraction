using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour {
    private Rigidbody2D rb;

    public float moveSpeed;
    private Vector2 _moveInput;
    public int energy = 100;
    public int maxEnergy = 100;
    public int skillTCost = 50;
    public float skillTCooldown = 1f;
    private float _lastSkillTTime;
    
    private float _timer = 0f;
    private bool _skillTTriggered;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        AwakeAnimation();
    }

    private void Update() {
        // Skill T
        if (isMoving && energy > skillTCost && Input.GetKeyDown(KeyCode.T) && Time.time - _lastSkillTTime > skillTCooldown) {
            energy -= skillTCost;
            _skillTTriggered = true;
            _lastSkillTTime = Time.time;
        }
        // Animation
        UpdateMovement();
        UpdateAnimation();
        // Energy Update
        _skillTTriggered = false;
        if (energy < maxEnergy) {
            _timer += Time.deltaTime;
            if (_timer >= 0.2f) {
                energy++;
                _timer = 0f;
            }
        }
        // TODO: 换一个开销小的实现
        var energyText = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        energyText.text = "Energy:" + energy;
    }
    
    private IEnumerator IncreaseEnergy() {
        while (energy < maxEnergy) {
            energy++;
            yield return new WaitForSeconds(1f);
        }
    }

    private void FixedUpdate() {
        Move();
    }

    private void UpdateMovement() {
        var move_input_x = Input.GetAxisRaw("Horizontal");
        var move_input_y = Input.GetAxisRaw("Vertical");
        var is_shift_down = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        _moveInput = new Vector2(move_input_x, move_input_y).normalized;
        if (is_shift_down) _moveInput *= 0.5f;

        isMoving = _moveInput != Vector2.zero;
    }

    private void Move() {
        rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * _moveInput);
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
            if (_skillTTriggered) animator.SetBool("isSkillPlaying", true);
            if (isMoving) {
                // 使用isMoving是为了让角色停止时，仍然会面朝移动方向
                animator.SetFloat("inputX", _moveInput.x);
                animator.SetFloat("inputY", _moveInput.y);
            }
        }
    }
}