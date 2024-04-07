using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;


    public int ammo;
    public int coin;
    public int health;

    public int maxammo;
    public int maxcoin;
    public int maxhealth;
    public int maxhasGrenades;

    float hAxis;
    float vAxis;


    bool WDown;
    bool jDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponindex = -1;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        
    }

  
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interation();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        WDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap)
            moveVec = Vector3.zero;
        transform.position += moveVec * speed * (WDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", WDown);
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

    }
    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump&&!isDodge && !isSwap)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump",true);
            anim.SetTrigger("doJump");

            isJump = true;
        }
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge &&!isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetBool("isJump", true);   
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut",0.5f);

        }
    }
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    } 
    void SwapOut()
    {
        isSwap = false;
    }
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponindex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponindex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponindex == 2))
            return;

        int weaponindex = -1;
        if (sDown1) weaponindex = 0;
        if (sDown2) weaponindex = 1;
        if (sDown3) weaponindex = 2;
        if ((sDown1||sDown2||sDown3) && !isJump &&! isDodge) {
            if (equipWeapon != null)
            equipWeapon.SetActive(false);

            equipWeaponindex = weaponindex;
            equipWeapon = weapons[weaponindex];
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }
    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                item items = nearObject.GetComponent<item>();
                int weaponindex = items.value;
                hasWeapons[weaponindex] = true;

                Destroy(nearObject);
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.tag == "Floor") {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
     void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            item items = other.GetComponent<item>();
            switch (items.type)
            {
                case item.Type.Ammo:
                    ammo += items.value;
                    if (ammo > maxammo)
                        ammo = maxammo;
                    break;
                case item.Type.Coin:
                    coin += items.value;
                    if (coin > maxcoin)
                        coin = maxcoin;
                    break;
                case item.Type.Heart:
                    health += items.value;
                    if (health > maxhealth)
                        health = maxhealth;
                    break;
                case item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += items.value;
                    if (hasGrenades > maxhasGrenades)
                        hasGrenades = maxhasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag=="Weapon")
        {
            nearObject = other.gameObject;

            Debug.Log(nearObject.name);
        }   
    }
    void OnTriggerExit(Collider other)
    {

            nearObject = null;

    }
}
