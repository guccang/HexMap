using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HexUnit : MonoBehaviour {
    const float rotationSpeed = 180f;
    const int speed = 5;
    private List<HexCell> pathToTravel;
    private Animator animator;
    private HexCell location;
    private float orientation;

    /// ////////////////////////////////////////
    public enum AnimationActionState
    {
        stand = 0,
        walk = 1,
    }
	public float travelSpeed ;
    public AnimationActionState State;
    public static HexUnit unitPrefab;
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                location.Unit = null;
            }
            location = value;
            value.Unit = this;
            transform.localPosition = value.Position;
        }
    }
    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }
    public int Speed
    {
        get
        {
            return speed;
        }
    }


    /// ////////////////////////////////////////
    public void Start()
    {
        animator = GetComponent<Animator>();
		animator = GetComponentInChildren<Animator>();
        travelSpeed = 1.0f;
    }
	public void ValidateLocation () {
		transform.localPosition = location.Position;
	}
	public bool IsValidDestination (HexCell cell) {
		return !cell.IsUnderwater && !cell.Unit;
	}
	public void Travel (List<HexCell> path) {
        // 设置了translate.localPosition位置后，startCoroutine中会再次设置为path的起始点
        Location = path[path.Count - 1];
        pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath());
    }
	public void Die () {
		location.Unit = null;
		Destroy(gameObject);
	}
	public void Save (BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(orientation);
	}
	public static void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddUnit(
			Instantiate(unitPrefab), grid.GetCell(coordinates), orientation
		);
	}
    public void Move()
    {
        animator.SetInteger("state", (int)AnimationActionState.walk);
        State = AnimationActionState.walk;

    }


    /// ////////////////////////////////////////
    private void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
		}
	}
	private void StopMove()
	{
		animator.SetInteger("state",(int)AnimationActionState.stand);
        State = AnimationActionState.stand;
        HexGameUI.OnStopMove();
	}
    private IEnumerator TravelPath()
    {
        Vector3 a, b, c = pathToTravel[0].Position;
        transform.localPosition = c;
        yield return LookAt(pathToTravel[1].Position);

        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + pathToTravel[i].Position) * 0.5f;
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                if (d.magnitude > 0)
                    transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            t -= 1f;
        }

        a = c;
        b = pathToTravel[pathToTravel.Count - 1].Position;
        c = b;
        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = location.Position;
        orientation = transform.localRotation.eulerAngles.y;
        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
        StopMove();
    }
    private IEnumerator LookAt(Vector3 point)
    {
        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =
            Quaternion.LookRotation(point - transform.localPosition);
        float speed = rotationSpeed / Quaternion.Angle(fromRotation, toRotation);

        for (
            float t = Time.deltaTime * speed;
            t < 1f;
            t += Time.deltaTime * speed
        )
        {
            transform.localRotation =
                Quaternion.Slerp(fromRotation, toRotation, t);
            yield return null;
        }

        transform.LookAt(point);
        orientation = transform.localRotation.eulerAngles.y;
    }

    //	void OnDrawGizmos () {
    //		if (pathToTravel == null || pathToTravel.Count == 0) {
    //			return;
    //		}
    //
    //		Vector3 a, b, c = pathToTravel[0].Position;
    //
    //		for (int i = 1; i < pathToTravel.Count; i++) {
    //			a = c;
    //			b = pathToTravel[i - 1].Position;
    //			c = (b + pathToTravel[i].Position) * 0.5f;
    //			for (float t = 0f; t < 1f; t += 0.1f) {
    //				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //			}
    //		}
    //
    //		a = c;
    //		b = pathToTravel[pathToTravel.Count - 1].Position;
    //		c = b;
    //		for (float t = 0f; t < 1f; t += 0.1f) {
    //			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //		}
    //	}
}