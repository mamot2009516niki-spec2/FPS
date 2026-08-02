using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // オブジェクトの変数
    public Camera CM; // カメラ
    public Transform viewPoint; // カメラの位置オブジェクト

    public Transform groundCheckPoint; // 地面に向けてRayを飛ばすオブジェクト
    public LayerMask groundLayers; // 地面だと認識するレイヤー
    public Rigidbody RB; // Rigidbody


    // 入力変数
    Vector2 mouseInput; // マウス入力
    float horizontalMouseInput; // x軸の回転
    float verticalMouseInput; // y軸の回転

    Vector3 movekeyInput; // 移動キーの入力
    Vector3 moveDirection; // 進む方向

    // ステータス変数
    public float mouseSensitivity = 1f; // 視点の移動速度
    public float moveSpeed = 4f; // プレイヤーの移動速度
    public float walkSpeed = 4f, runSpeed = 8f; // プレイヤーの歩く速度と走る速度
    public float jumpForce = 6f; // プレイヤーのジャンプ力

    // 効果音変数
    AudioSource landAudioSource;
    AudioSource jumpAudioSource;
    bool isWalking; // 歩いているか判定
    public AudioClip jumpSound; // ジャンプ音
    public AudioClip landSound; // 足音

    // カメラの上下動に関する変数
    float stepCounter; // 歩数カウンター
    public float bobSpeed = 10f; // 上下動の速度
    public float bobAmount = 0.1f; // 上下動の量
    Vector3 viewPointStartPosition; // カメラの初期位置
    float bobbingOffset; // 現在の上下動のオフセット

    void Start()
    {
        CM = Camera.main; // カメラを変数に代入
        viewPointStartPosition = viewPoint.transform.localPosition; // プレイヤーの視点を決めるオブジェクトを変数に代入
        RB = GetComponent<Rigidbody>(); // Rigidbodyを変数に代入
        landAudioSource = GetComponent<AudioSource>(); // 足音を変数に代入
        jumpAudioSource = GetComponent<AudioSource>(); // ジャンプ音を変数に代入
    }

    void Update()
    {
        RotatePlayer(); // 視点の移動関数
        MovePlayer(); // プレイヤーの移動関数
        ControlPlayerSpeed(); // プレイヤーの移動速度制御関数
        JumpPlayer(); // プレイヤーのジャンプ関数
        UpdateWalkingSound(); // 足音を再生する関数

        // 歩数に合わせて視点カメラを滑らかに上下させる処理
        if (IsGround() && moveDirection.magnitude > 0)
        {
            // 歩行を始めてからの時間×上下動の速度を歩行カウンターに代入
            stepCounter += Time.deltaTime * bobSpeed;

            // sin波を使ってPlayerの視点の高さ(y軸)を滑らかに上下に動かす処理
            bobbingOffset = Mathf.Sin(stepCounter) * bobAmount;
            viewPoint.transform.localPosition = viewPointStartPosition + new Vector3(0, bobbingOffset, 0);
        }
        else
        {
            stepCounter = 0; // 歩行していない場合はカウンターをリセット
            viewPoint.transform.localPosition = Vector3.Lerp(viewPoint.transform.localPosition, viewPointStartPosition, Time.deltaTime * bobSpeed); //初期位置に滑らかに戻す
        }
    }

    // プレイヤーの視点を移動先の位置と向きに反映させる処理
    void LateUpdate()
    {
        CM.transform.position = viewPoint.position;
        CM.transform.rotation = viewPoint.rotation;
    }

    // プレイヤーの着地判定関数
    public bool IsGround()
    { 
        // 自分の足元から下に25cm分Raycastを飛ばして、地面のレイアーに当たればTrueを返す
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers); 
    }

    // 視点の移動関数
    public void RotatePlayer()
    {
        // 変数にマウスの動きを代入する
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity,Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        // 変数にx軸のマウス入力分の数値を足す
        horizontalMouseInput += mouseInput.x;

        // 変数にy軸のマウス入力分の数値を足す
        verticalMouseInput += mouseInput.y;

        // 変数の数値を丸める（上下の視点制御）
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);

        // 視点の横回転
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, horizontalMouseInput, transform.eulerAngles.z);

        // 視点の縦回転
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput, viewPoint.transform.rotation.eulerAngles.y, viewPoint.transform.rotation.eulerAngles.z);
    }

    // プレイヤーの移動関数
    public void MovePlayer()
    {
        // 変数に水平垂直の移動キー(WASD+矢印キー)の動きを代入する
        movekeyInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // プレイヤーの移動方向を代入する
        moveDirection = ((transform.forward * movekeyInput.z) + (transform.right * movekeyInput.x)).normalized;

        // 今の位置に移動方向と移動スピードと前回の移動からの時間を掛け合わせた値を加える
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    // プレイヤーの移動速度制御関数
    public void ControlPlayerSpeed()
    {
        // Shiftキーが押されているときは走っている状態にする
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveSpeed = runSpeed; // 移動スピードに走っているときのスピードを代入
            landAudioSource.pitch = 1.2f; // 足音のピッチを1.2倍にする
        }
        // Shiftキーが押されていないときは歩いている状態にする
        else
        {
            moveSpeed = walkSpeed; // 移動スピードに歩いているときのスピードを代入
            landAudioSource.pitch = 1.0f; // 足音のピッチを1.0倍にする
        }
    }

    // 足音を再生する関数
    private void UpdateWalkingSound()
    {
        // 歩いているかどうかを判断し、足音を再生
        if (IsGround() && moveDirection.magnitude > 0 && !landAudioSource.isPlaying)
        {
            landAudioSource.PlayOneShot(landSound);
        }
    }

    // プレイヤーのジャンプ関数
    public void JumpPlayer()
    {
        // 地面への着地判定があり、Spaceキーが押されたとき
        if (IsGround() && Input.GetKeyDown(KeyCode.Space))
        {
            // ジャンプ音を再生する
            jumpAudioSource.PlayOneShot(jumpSound);
            // 瞬間的な真上への力を加える
            RB.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        }
    }

}
