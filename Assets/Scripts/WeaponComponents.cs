using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class WeaponComponents : MonoBehaviour {

	public Sprite[] modules;
	
	private Weapon parent;
	private SpriteRenderer spriteRenderer;
	public int Power;
	
	void Start () {
		parent = GetComponentInParent<Weapon> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		Power = Random.Range(0, modules.Length);
        spriteRenderer.sprite = modules [Power];
	}

	void Update () {
		transform.eulerAngles = parent.transform.eulerAngles;
	}
	
	public SpriteRenderer getSpriteRenderer () {
		return spriteRenderer;
	}
}
