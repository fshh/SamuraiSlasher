using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DuelManager : MonoBehaviour {
    public GameObject duelPrompt;
    public GameObject duelAnimeEffect;
    public AudioClip duelStart, promptSound, duelEnd;
    public float zoomDuration = 0.15f;
    public float duelDistance = 14f;
    public float cameraPadding = 5f;
    public float promptDelay = 1f;
    public float promptWidth = 200f;

    private enum ControllerButton { A = 0, B = 1, X = 2, Y = 3 };
    private Dictionary<ControllerButton, string> buttonStrings = new Dictionary<ControllerButton, string>() {
        { ControllerButton.A, "360_A" }, { ControllerButton.B, "360_B" }, { ControllerButton.X, "360_X" }, { ControllerButton.Y, "360_Y" }
    };

    private ControllerButton promptButton;
    private bool promptInputAllowed = false;

    private GameObject participant1 = null;
    private GameObject participant2 = null;

    private int p1Num = 0;
    private int p2Num = 0;

    private AudioSource audioSource;
    private Vector3 duelMidpoint = Vector3.zero;
    private float normalCameraSize;
    private bool duelIsHappening = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        normalCameraSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (promptInputAllowed) {
            if (Input.GetButtonDown("A_P" + p1Num)) {
                GivePromptInput(ControllerButton.A, participant1, participant2);
            } else if (Input.GetButtonDown("B_P" + p1Num)) {
                GivePromptInput(ControllerButton.B, participant1, participant2);
            } else if (Input.GetButtonDown("X_P" + p1Num)) {
                GivePromptInput(ControllerButton.X, participant1, participant2);
            } else if (Input.GetButtonDown("Y_P" + p1Num)) {
                GivePromptInput(ControllerButton.Y, participant1, participant2);
            } else if (Input.GetButtonDown("A_P" + p2Num)) {
                GivePromptInput(ControllerButton.A, participant2, participant1);
            } else if (Input.GetButtonDown("B_P" + p2Num)) {
                GivePromptInput(ControllerButton.B, participant2, participant1);
            } else if (Input.GetButtonDown("X_P" + p2Num)) {
                GivePromptInput(ControllerButton.X, participant2, participant1);
            } else if (Input.GetButtonDown("Y_P" + p2Num)) {
                GivePromptInput(ControllerButton.Y, participant2, participant1);
            }
        }
    }

    // determines whether the specified input is correct or not, and resolves the duel accordingly
    private void GivePromptInput(ControllerButton button, GameObject player, GameObject enemy) {
        // prevent any more prompt input
        promptInputAllowed = false;

        // remove duel prompt
        duelPrompt.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        DashAttack killer;
        Vector3 killedPos;
        if (button == promptButton) {
            killer = player.GetComponent<DashAttack>();
            enemy.GetComponent<PlayerDeath>().Die();
            killedPos = enemy.transform.position;
        } else {
            killer = enemy.GetComponent<DashAttack>();
            player.GetComponent<PlayerDeath>().Die();
            killedPos = player.transform.position;
        }

        killer.Attack(duelDistance);
        killer.GetComponent<ChargeManager>().addCharge(100);

        killedPos.z = Camera.main.transform.position.z;
        float killDuration = killer.dashDuration + killer.slashDuration;

        // play kill in slowmotion
        Time.timeScale = 0.5f;

        Sequence cameraSequence = DOTween.Sequence();

        // move camera over player who is about to die
        Tweener cameraMove = Camera.main.transform.DOMove(killedPos, killDuration / 4f)
            .OnComplete(()=> audioSource.PlayOneShot(duelEnd));

        // add camera shake
        Tweener cameraShake = Camera.main.transform.DOShakePosition(3f * (killDuration / 4f), new Vector3(5f, 5f, 0f), vibrato: 20)
            .OnKill(EndDuel); // end the duel after watching the kill

        cameraSequence.Append(cameraMove).Append(cameraShake);
        cameraSequence.Play();
    }

    public void CreateDuel(GameObject p1, GameObject p2) {
        // don't start a duel if one is already happening
        if (duelIsHappening) {
            return;
        }

        // stop time for all players
        Time.timeScale = 0f;

        // disable input for all players
        InputManager[] inputs = FindObjectsOfType<InputManager>();
        foreach (InputManager input in inputs) {
            input.canReceiveInput = false;
        }

        // remember that a duel is happening
        duelIsHappening = true;

        // remember players who are engaged in the duel so we can give them back input control
        participant1 = p1;
        participant2 = p2;

        // get player numbers so we can get their input for the button prompt
        p1Num = (int)participant1.GetComponent<InputManager>().playerNumber;
        p2Num = (int)participant2.GetComponent<InputManager>().playerNumber;

        // put the participants in an array to perform repeated operations
        GameObject[] participants = new GameObject[2] { participant1, participant2 };

        // find the midpoint around which the players will duel
        duelMidpoint = (participant1.transform.position + participant2.transform.position) / 2f;

        // disable both players' input and align them to face each other equidistant around a midpoint
        foreach (GameObject obj in participants) {
            // get vector from player position to midpoint
            Vector3 vectorToDuelMidpoint = obj.transform.position - duelMidpoint;

            // determine angle to face midpoint
            float angle = Mathf.Atan2(vectorToDuelMidpoint.y, vectorToDuelMidpoint.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(angle + 90, Vector3.forward);

            // move to be equidistant from the midpoint in comparison to other player
            obj.transform.DOMove(vectorToDuelMidpoint.normalized * (duelDistance / 2f) + duelMidpoint, zoomDuration).SetEase(Ease.OutQuart).SetUpdate(true);

            // face the other player head-on
            obj.transform.DORotateQuaternion(rot, zoomDuration).SetEase(Ease.OutQuart).SetUpdate(true);

            // disable overhead player number
            obj.transform.Find("AbovePlayerCanvas").Find("PlayerNumberCenter").gameObject.SetActive(false);
        }
        
        // get point to which camera will move
        Vector3 cameraCenter = duelMidpoint;
        cameraCenter.z = Camera.main.transform.position.z;

        // move camera to zoom in and focus around duel
        Camera.main.transform.DOMove(cameraCenter, zoomDuration).SetEase(Ease.OutQuart).SetUpdate(true);
        DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, (duelDistance / 2f) + cameraPadding, zoomDuration).SetEase(Ease.OutQuart).SetUpdate(true);

        // activate visual effect around edges of screen
        duelAnimeEffect.SetActive(true);

        // pause main music
        GameObject.Find("MusicPlayer").GetComponent<AudioSource>().Pause();

        // play sword clash sound effect
        audioSource.PlayOneShot(duelStart);

        // display button prompt after delay
        promptButton = (ControllerButton)Random.Range(0, 4);
        duelPrompt.GetComponent<Image>().sprite = Resources.Load<Sprite>(buttonStrings[promptButton]);
        RectTransform duelPromptTransform = duelPrompt.GetComponent<RectTransform>();
        DOTween.To(()=> duelPromptTransform.sizeDelta, x=> duelPromptTransform.sizeDelta = x, new Vector2(promptWidth, promptWidth), 0.05f)
            .SetDelay(promptDelay).OnStart(AllowPromptInput).SetUpdate(true);
    }

    private void AllowPromptInput() {
        audioSource.PlayOneShot(promptSound);
        promptInputAllowed = true;
    }

    public void EndDuel() {
        // re-enable input for all players
        InputManager[] inputs = FindObjectsOfType<InputManager>();
        foreach (InputManager input in inputs) {
            input.canReceiveInput = true;
        }

        GameObject[] participants = new GameObject[2] { participant1, participant2 };

        foreach (GameObject obj in participants) {
            // enable overhead player number
            obj.transform.Find("AbovePlayerCanvas").Find("PlayerNumberCenter").gameObject.SetActive(true);
        }

        // reset global variables
        participant1 = null;
        participant2 = null;
        p1Num = 0;
        p2Num = 0;
        duelMidpoint = Vector3.zero;
        promptInputAllowed = false;

        // move camera back to center
        Vector3 cameraCenter = Vector3.zero;
        cameraCenter.z = Camera.main.transform.position.z;

        // zoom out more slowly than we zoomed in to start the duel
        float zoomOutDuration = zoomDuration * 3;

        // move the camera back to the center of the screen and zoom out
        Camera.main.transform.DOMove(cameraCenter, zoomOutDuration).SetEase(Ease.OutQuart).SetUpdate(true);
        DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, normalCameraSize, zoomOutDuration).SetEase(Ease.OutQuart).SetUpdate(true);

        // remember that a duel is no longer happening
        duelIsHappening = false;

        // resume normal game speed
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, zoomOutDuration).SetEase(Ease.OutQuart).SetUpdate(true);

        // deactivate visual effect around edges of screen
        duelAnimeEffect.SetActive(false);

        // unpause main music
        GameObject.Find("MusicPlayer").GetComponent<AudioSource>().UnPause();
    }
}
