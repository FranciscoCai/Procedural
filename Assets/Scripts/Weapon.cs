using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour {

	public bool inPlayerInventory = false;
	public int[] WeaponPower;
	private Player player;
	private WeaponComponents[] weaponsComps;
	private bool weaponUsed = false;
	public SpriteRenderer spriteRenderer;
	private int randomLevel;
    [SerializeField] float incrementVelocity;
    [SerializeField] int limiteAzul;
    [SerializeField] int limiteVerde;
    [SerializeField] int limitePurpura;
    [SerializeField] int limiteAmarilla;


    public void AquireWeapon () {
		player = GetComponentInParent<Player> ();
		
        WeaponPower = new int[weaponsComps.Length];
        for (int i = 0; i < weaponsComps.Length; i++)
        {
            WeaponPower[i] = weaponsComps[i].Power;
        }

        if (randomLevel >= 0 && randomLevel < limiteAzul)
        {
            WeaponPower[0] += 5;
        }
        else if (randomLevel >= limiteAzul && randomLevel < limiteVerde)
        {
            WeaponPower[0] += 10;
        }
        else if (randomLevel >= limiteVerde && randomLevel < limitePurpura)
        {
            WeaponPower[0] += 20;
        }
        else
        {
            WeaponPower[0] += 50;
        }
    }
    private void Start()
    {
        weaponsComps = GetComponentsInChildren<WeaponComponents>();
        int nivelMaximo = Convert.ToInt32(Math.Min(GameManager.instance.playerLevel, 100f));
        randomLevel = Random.Range(0, nivelMaximo);
        spriteRenderer = weaponsComps[0].GetComponent<SpriteRenderer>();
        if (randomLevel >= 0 && randomLevel < limiteAzul)
        {
            spriteRenderer.color = Color.blue;
        }
        else if (randomLevel >= limiteAzul && randomLevel < limiteVerde)
        {
            spriteRenderer.color = Color.green;
        }
        else if (randomLevel >= limiteVerde && randomLevel < limitePurpura)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = Color.magenta;
        }
    }

    void Update () {
		if (inPlayerInventory) {
			transform.position = player.transform.position;
			if (weaponUsed == true) {
				float degreeY = 0, degreeZ = -90f, degreeZMax = 275f;
				Vector3 returnVecter = Vector3.zero;
				if (Player.isFacingRight) {
					degreeY = 0;
					returnVecter = Vector3.zero;
				} else if (!Player.isFacingRight) {
					degreeY = 180;
					returnVecter = new Vector3(0,180,0);
				}
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, degreeY, degreeZ), Time.deltaTime * 20f);
				if (transform.eulerAngles.z <= degreeZMax) {
					transform.eulerAngles = returnVecter;
					weaponUsed = false;
					enableSpriteRender (false);
				}
			}
		}
	}
	
	public void useWeapon () {
		enableSpriteRender(true);
		weaponUsed = true;
	}

	public void enableSpriteRender (bool isEnabled) {
		foreach (WeaponComponents comp in weaponsComps) {
			comp.getSpriteRenderer ().enabled = isEnabled;
		}
	}

	public Sprite getComponentImage (int index) {
		return weaponsComps[index].getSpriteRenderer().sprite;
	}

}
