using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    public Transform acornHolder;
    private AcornPickup currentAcorn;
    private float pickupTime;
    private float deliveryDelay = 0.2f;

    public void PickUpAcorn(AcornPickup acorn)
    {
        currentAcorn = acorn;
        acorn.transform.SetParent(acornHolder);
        acorn.transform.localPosition = Vector3.zero;
        acorn.transform.localRotation = Quaternion.identity;
        acorn.gameObject.SetActive(true);

        pickupTime = Time.time;

        Debug.Log("[PlayerCarry] �o��d� podniesiony!");

    }

    public bool CanDeliverAcorn()
    {
        return currentAcorn != null && Time.time - pickupTime >= deliveryDelay;
    }

    public bool HasAcorn()
    {
        return currentAcorn != null;
    }

    public void DropAcornAtCheckpoint()
    {
        if (currentAcorn != null)
        {
            Debug.Log("[PlayerCarry] DropAcornAtCheckpoint() � respawnujemy �o��d�");
            currentAcorn.RespawnAtCheckpoint();
            currentAcorn = null;
        }
        else
        {
            Debug.Log("[PlayerCarry] Brak �o��dzia do zrzucenia.");
        }
    }

    public void DropAcornAtGoal()
    {
        if (currentAcorn != null)
        {
            Destroy(currentAcorn.gameObject);
            currentAcorn = null;
            Debug.Log("[PlayerCarry] �o��d� dostarczony i usuni�ty.");
        }
    }
}