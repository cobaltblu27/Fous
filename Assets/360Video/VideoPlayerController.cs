using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float skipTime;


    private InputAction pauseAction;
    private InputAction rewindAction;
    private InputAction forwardAction;

    public InputActionProperty someButtonAction; // primary button

    private void OnEnable()
    {
        // 키보드 'space' 키 입력에 대한 액션 생성 및 바인딩
        pauseAction = new InputAction(binding: "<Keyboard>/space", type: InputActionType.Button, interactions: "press");
        pauseAction.Enable();
        pauseAction.performed += context => ToggleVideoPlayback();

        // 특정 버튼에 대한 액션 생성 및 바인딩
        someButtonAction.action.performed += context => ToggleVideoPlayback();
        someButtonAction.action.Enable();

        // 시간 조정
        rewindAction = new InputAction(binding: "<Keyboard>/leftArrow", type: InputActionType.Button, interactions: "press");
        rewindAction.Enable();
        rewindAction.performed += context => RewindVideo(skipTime); // 이전 5초 이동

        forwardAction = new InputAction(binding: "<Keyboard>/rightArrow", type: InputActionType.Button, interactions: "press");
        forwardAction.Enable();
        forwardAction.performed += context => ForwardVideo(skipTime); // 이후 5초 이동

    }

    private void OnDisable()
    {
        pauseAction.Disable();
        someButtonAction.action.Disable();
        rewindAction.Disable();
        forwardAction.Disable();
    }

    // 영상 재생/일시 정지 함수
    void ToggleVideoPlayback()
    {
        if (videoPlayer)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause(); // 영상 일시 정지
            }
            else
            {
                videoPlayer.Play(); // 영상 재생
            }
        }
    }

    // skipTime 전으로 이동
    void RewindVideo(float timeToRewind)
    {
        if (videoPlayer)
        {
            videoPlayer.time = Mathf.Max((float)videoPlayer.time - timeToRewind, 0f);
        }
    }

    // skipTime 후로 이동
    void ForwardVideo(float timeToForward)
    {
        if (videoPlayer)
        {
            videoPlayer.time = Mathf.Min((float)videoPlayer.time + timeToForward, (float)videoPlayer.length);
        }
    }
}
