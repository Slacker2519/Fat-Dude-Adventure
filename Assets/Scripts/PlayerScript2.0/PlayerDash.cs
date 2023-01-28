using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    PlayerMovementManager _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerMovementManager>();
    }

    public void Dash(float dashVelocity)
    {
        _playerController.Rb.gravityScale = 0f;
        _playerController.Rb.velocity = new Vector2(_playerController.transform.localScale.x * dashVelocity, 0f);
    }

    public IEnumerator SpawnAfterImage(GameObject playerAfterImagePrefabs, float afterImageSpawnDelay)
    {
        while (true)
        {
            Instantiate(playerAfterImagePrefabs, _playerController.transform.position, Quaternion.identity).transform.localScale = _playerController.transform.localScale;
            yield return new WaitForSeconds(afterImageSpawnDelay);
        }
    }
}
