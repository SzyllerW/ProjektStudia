using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    public Transform acornHolder;
    private AcornPickup currentAcorn;

    public void PickUpAcorn(AcornPickup acorn)
    {
        currentAcorn = acorn;
        acorn.transform.SetParent(acornHolder);
        acorn.transform.localPosition = Vector3.zero;
        acorn.transform.localRotation = Quaternion.identity;
        acorn.gameObject.SetActive(true);
    }

    public bool HasAcorn()
    {
        return currentAcorn != null;
    }

    public void DropAcornAtCheckpoint()
    {
        if (currentAcorn != null)
        {
            currentAcorn.RespawnAtCheckpoint();
            currentAcorn = null;
        }
    }
}