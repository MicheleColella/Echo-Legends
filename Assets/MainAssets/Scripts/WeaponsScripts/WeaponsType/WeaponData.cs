// WeaponData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Vector2 damageRange; // Range del danno dell'arma
    public float fireRate; // Rateo di fuoco (proiettili per secondo)
    public GameObject projectilePrefab; // Prefab del proiettile
    public float projectileSpeed; // Velocità del proiettile
    public int projectilesPerShot; // Numero di proiettili sparati per ogni colpo
    [Range(0, 360)]
    public float spreadAngle; // Angolo di dispersione dei proiettili
    public float recoilForce; // Forza di rinculo
}