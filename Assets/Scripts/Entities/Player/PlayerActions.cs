using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// The PlayerActions class is responsible for handling the player's actions.
/// </summary>
public class PlayerActions : MonoBehaviour
{
    /// <summary>
    /// The player property is responsible for storing the player's Rigidbody2D component.
    /// </summary>
    private Rigidbody2D player;

    /// <summary>
    /// The gate property is responsible for storing the gate GameObject.
    /// </summary>


    /// <summary>
    /// The layer property is responsible for storing the layer mask.
    /// This layer mask will be used to check the layer of the objects and items near the player.
    /// </summary>
    private LayerMask layer;

    /// <summary>
    /// The gate and objectNear properties are responsible for storing the gate and object near the player.
    /// </summary>
    private GameObject gate, objectNear;

    /// <summary>
    /// The playerMaxHealth property is responsible for storing the player's maximum health.
    /// </summary>
    private int playerMaxHealth;

    /// <summary>
    /// The Awake method is called when the script instance is being loaded (Unity Method).
    /// In this method, we are initializing the player and gate variables and setting the HasKey property to false.
    /// </summary>
    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
        gate = GameObject.Find("Gate");
        layer = LayerMask.GetMask("Default");
        playerMaxHealth = GetComponent<Entity>().Health;
    }

    /// <summary>
    /// The Update method is called every frame (Unity Method).
    /// In this method, we are checking if the are gates, if the player pressed the E key, and if the heal conditions are met.
    /// If there are gates, the IsGateNear() and OpenGate() methods are called.
    /// If the player pressed the E key, the CheckGrabObjectsConditions() method is called.
    /// And if the player pressed the H key, the HealPlayer() method is called.
    /// </summary>
    private void Update()
    {
        if (gate != null)
        {
            if (Input.GetKeyDown(KeyCode.E) && IsGateNear())
            {   
                OpenGate();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckGrabObjectsConditions();
        }

        // Heal Conditions
        if (Input.GetKeyDown(KeyCode.H) && GetComponent<PlayerInventory>().Items["HealItems"] > 0)
        {
            HealPlayer();
        }
    }

    /// <summary>
    /// The CheckGrabObjectsConditions method is responsible for checking the conditions to grab an object (item or weapon).
    /// The first if stament condition is to check the conditions to grab an item, the second condition is to check the conditions to grab a weapon.
    /// </summary>
    private void CheckGrabObjectsConditions()
    {   
        bool grabItemsConditions = ItemsLeft() && IsObjectNear("Item");

        if (grabItemsConditions|| IsObjectNear("Weapon"))
        {
            GrabObject();
        }
    }

    /// <summary>
    /// The isGateNear method is responsible for checking if the player is near the gate.
    /// A raycast is create to check if the player is near the gate and its facing down the gate.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the gate is near otherwise, <c>false</c>.
    /// </returns>
    private bool IsGateNear()
    {        
        float rayCastDistance = 1.5f;

        Vector2 raycastOrigin = (Vector2)player.position + new Vector2(0, -player.GetComponent<Collider2D>().bounds.extents.y);

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, rayCastDistance, layer);

        // This line is used to visualize the raycast in the scene view, for debugging purposes.
        Debug.DrawRay(raycastOrigin, Vector2.down * rayCastDistance, Color.red);

        return hit.collider != null && hit.collider.gameObject.name == "Gate";

    }

    /// <summary>
    /// The IsObjectNear method is responsible for checking if the player is near an object, the object can be an item or a weapon.
    /// To check if the player is near an object, a box collider is created around the player.
    /// If the player is near an  object, the objectNear property is updated with the object.
    /// </summary>
    /// <param name="objectTag">The object tag.</param>
    /// <returns>
    ///   <c>true</c> if the object is  near; otherwise, <c>false</c>.
    /// </returns>
    private bool IsObjectNear(string objectTag)
    {   
        BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();

        // The box size is 1.5 times the player's collider size.
        Vector2 boxSize = playerCollider.bounds.size * 1.5f;

        Vector2 offsetPosition = (Vector2)playerCollider.bounds.center;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(offsetPosition, boxSize, 0, layer);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag(objectTag))
            {          
                objectNear = collider.gameObject;
                
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The OpenGate method is responsible for opening the gate, if the player has the key.
    /// If the player has the corect key, the gate is opened, otherwise the key is removed from the player's inventory.
    /// To check if the player has the correct key, it checks the keys dictionary of the first level.
    /// If the keys dictionary has no key with a true value, it means that the player has grabbed the correct key.
    /// </summary>
    private void OpenGate()
    {
        if (GetComponent<PlayerInventory>().Items["Key"] == 1)
        {
            // Gets the key and its value
            Dictionary<GameObject, bool> keys = GameObject.Find("Level1").GetComponent<Level1Logic>().Keys;

            if (!keys.Values.Any(value => value))
            {    
                // Opens the gate
                 gate.SetActive(false);
            }
            else
            {
                // Removes the key from the player's inventory
                GetComponent<PlayerInventory>().Items["Key"] = 0;
            }
        }  
    }

    /// <summary>
    /// The ItemsLeft method is responsible for checking if there are items left in the level.
    /// </summary>
    /// <returns></returns>
    private bool ItemsLeft()
    {
        return GameObject.FindGameObjectsWithTag("Item").Length > 0;
    }

    /// <summary>
    /// The GrabObject method is responsible for grabbing the object near the player.
    /// It removes the (Clone) string from the object's name, if it exists, to get the original object name.
    /// It calls the correct method to grab the object, depending on the object's name.
    /// </summary>
    private void GrabObject()
    {   
        if (objectNear.name.Contains("(Clone"))
        {
          objectNear.name = objectNear.name.Replace("(Clone)", "");
        }

        switch (objectNear.name)
        {
            case "Stick":
            case "Sword":
                 GrabMeleeWeapon();

                return;

            case "Key":
                Debug.Log("Grab Key");
                GrabKey();

                return;

            case "HealItem":
                GrabHealItem();

                return;

            default:
                return;
        }      
    }

    /// <summary>
    /// The GrabMeleeWeapon method is responsible for grabbing the melee weapon, updating the player's inventory, and removing the weapon from the level.
    /// If the player grabs the stick, PlayerAttack component is enable, because the stick is the player's first weapon.
    /// </summary>
    private void GrabMeleeWeapon()
    {   
        if (objectNear.name == "Stick")
        {
            GetComponent<PlayerAttack>().enabled = true;
        }

        GetComponent<PlayerInventory>().Weapons["Melee"] = objectNear.name;

        DestroyObject();
    }

    /// <summary>
    /// The GrabKey method is responsible for grabbing the key.
    /// It adds a key to the player's inventory, and removes the key from the level.
    /// </summary>
    private void GrabKey()
    {   
        if (GetComponent<PlayerInventory>().Items["Key"] == 0)
        {  
            GetComponent<PlayerInventory>().Items["Key"] = 1;

            GameObject itemToDestroy = DestroyObject();

            // Removes the key from the dictionary which stores the keys and their values
            GameObject.Find("Level1").GetComponent<Level1Logic>().Keys.Remove(itemToDestroy);

            SpawnHordes();
        }   
    }

    /// <summary>
    /// The GrabHealItem method is responsible for grabbing the heal item, if the player has space in the inventory.
    /// If the player has space in the inventory, the player's inventory is updated, and the item is destroyed.
    /// </summary>
    private void GrabHealItem()
    {
        if (GetComponent<PlayerInventory>().Items["HealItems"] < PlayerInventory.MaxHealItems)
        {
            GetComponent<PlayerInventory>().Items["HealItems"]++;

            DestroyObject();
        }
    }

    /// <summary>
    /// The DestroyObject method is responsible for destroying the object near the player.
    /// </summary>
    /// <returns>The object to destroy</returns>
    private GameObject DestroyObject()
    {
        GameObject objectToDestroy = objectNear;
        objectNear = null;
        Destroy(objectToDestroy);

        return objectToDestroy;
    }

    /// <summary>
    /// The HealPlayer method is responsible for healing the player, if the player's health is less than its maximum health.
    /// </summary>
    private void HealPlayer()
    {   
        if (GetComponent<Entity>().Health < playerMaxHealth)
        {
            GetComponent<Entity>().Health++;
            GetComponent<PlayerInventory>().Items["HealItems"]--;
        }
    }


    //abc
    private void SpawnHordes(){

        GameObject spawnHord = GameObject.Find("SpawnHorde");

        spawnHord.GetComponent<SpawnHorde>().enabled = true;

    }

}
