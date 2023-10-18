using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Set in Inspector: Enemy")]
    public float speed = 10f; // The speed in m/s
    public float fireRate = 0.3f; // Seconds/shot (Unused)
    public float health = 10;
    public int score = 100; // Points earned for destroying this
    public float showDamageDuration = 0.1f; // # seconds to show damage
    public float powerUpDropChance = 1f; // Chance to drop a power-up
    public GameObject shrapnelPrefab; //Shrapnel prefab upon death

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;// All the Materials of this & its children
    public bool showingDamage = false;
    public float damageDoneTime; // Time to stop showing damage
    public bool notifiedOfDestruction = false; // Will be used later

    protected BoundsCheck bndCheck;

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        // Get materials and colors for this GameObject and its children
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i=0; i<materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    // This is a property: A method that acts like a field
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();

        if(showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.offDown)
        {
            // We're off the bottom, so destroy this GameObject
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                // Hurt this Enemy
                ShowDamage();
                // Get the damage amount from the Main WEAP_DICT
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if(health <= 0)
                {
                    // Tell the Main singleton that this ship was destroyed
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    // Destroy this enemy
                    Destroy(this.gameObject);
                    //Instantiate 4 "shrapnel" prefabs that travel along each axis. Transform each shrapnel along its respective positive or negative x or y axis
                    GameObject shrapnel1 = Instantiate<GameObject>(shrapnelPrefab);
                    shrapnel1.transform.position = this.transform.position;
                    GameObject shrapnel2 = Instantiate<GameObject>(shrapnelPrefab);
                    shrapnel2.transform.position = this.transform.position;
                    GameObject shrapnel3 = Instantiate<GameObject>(shrapnelPrefab);
                    shrapnel3.transform.position = this.transform.position;
                    GameObject shrapnel4 = Instantiate<GameObject>(shrapnelPrefab);
                    shrapnel4.transform.position = this.transform.position;
                    //Shrapnel 1
                    shrapnel1.transform.Translate(Vector3.up * 0.60f);
                    shrapnel1.transform.Translate(Vector3.right * 0.60f);
                    //Add movement to the shrapnel
                    shrapnel1.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    shrapnel1.GetComponent<Rigidbody>().AddForce(Vector3.up * 1000);
                    shrapnel1.GetComponent<Rigidbody>().AddForce(Vector3.right * 1000);
                    //Shrapnel 2
                    shrapnel2.transform.Translate(Vector3.up * 0.60f);
                    shrapnel2.transform.Translate(Vector3.left * 0.60f);
                    //Add movement to the shrapnel
                    shrapnel2.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    shrapnel2.GetComponent<Rigidbody>().AddForce(Vector3.up * 1000);
                    shrapnel2.GetComponent<Rigidbody>().AddForce(Vector3.left * 1000);
                    //Shrapnel 3
                    shrapnel3.transform.Translate(Vector3.down * 0.60f);
                    shrapnel3.transform.Translate(Vector3.right * 0.60f);
                    //Add movement to shrapnel 3
                    shrapnel3.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    shrapnel3.GetComponent<Rigidbody>().AddForce(Vector3.down * 1000);
                    shrapnel3.GetComponent<Rigidbody>().AddForce(Vector3.right * 1000);
                    //Shrapnel 4
                    shrapnel4.transform.Translate(Vector3.down * 0.60f);
                    shrapnel4.transform.Translate(Vector3.left * 0.60f);
                    //Add movement to shrapnel 4
                    shrapnel4.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    shrapnel4.GetComponent<Rigidbody>().AddForce(Vector3.down * 1000);
                    shrapnel4.GetComponent<Rigidbody>().AddForce(Vector3.left * 1000);
                    // Destroy the ProjectileHero
                    Destroy(otherGO);
                }
                Destroy(otherGO);
                break;

            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for (int i=0; i<materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
