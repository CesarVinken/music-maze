using Photon.Pun;
using UnityEngine;

[HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
[RequireComponent(typeof(PhotonView))]
public class CustomisedPhotonTransformView : MonoBehaviour, IPunObservable
{
    private float m_Distance;
    private float m_Angle;

    private PhotonView m_PhotonView;

    private Vector3 m_Direction;
    private Vector3 m_NetworkPosition;
    private Vector3 m_StoredPosition;

    private Quaternion m_NetworkRotation;

    public bool m_SynchronizePosition = true;
    public bool m_SynchronizeRotation = true;
    public bool m_SynchronizeScale = false;

    public bool TeleportEnabled = true;
    public float TeleportIfDistanceGreaterThan = .9f;

    public void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        m_StoredPosition = transform.position;
        m_NetworkPosition = Vector3.zero;

        m_NetworkRotation = Quaternion.identity;
    }

    public void Update()
    {
        if (!this.m_PhotonView.IsMine)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));

            if (GameRules.GamePlayerType == GamePlayerType.Multiplayer && TeleportEnabled == true && Vector3.Distance(transform.position, this.m_NetworkPosition) > TeleportIfDistanceGreaterThan)
            {
                Logger.Log("The distance is too large. Teleport the player to {0},{1}.", this.m_NetworkPosition.x, this.m_NetworkPosition.y);
                transform.position = this.m_NetworkPosition;
            }
            else
            {
            transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (this.m_SynchronizePosition)
            {
                this.m_Direction = transform.position - this.m_StoredPosition;
                this.m_StoredPosition = transform.position;

                stream.SendNext(transform.position);
                stream.SendNext(this.m_Direction);
            }

            if (this.m_SynchronizeRotation)
            {
                stream.SendNext(transform.rotation);
            }

            if (this.m_SynchronizeScale)
            {
                stream.SendNext(transform.localScale);
            }
        }
        else
        {
            if (this.m_SynchronizePosition)
            {
                this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                this.m_Direction = (Vector3)stream.ReceiveNext();

                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
                this.m_NetworkPosition += this.m_Direction * lag;

                this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
            }

            if (this.m_SynchronizeRotation)
            {
                this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
            }

            if (this.m_SynchronizeScale)
            {
                transform.localScale = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
