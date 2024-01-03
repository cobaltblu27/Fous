using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private InputAction pauseAction;

    private void OnEnable()
    {
        // 키보드 'S' 키 입력에 대한 액션 생성 및 바인딩
        pauseAction = new InputAction(binding: "<Keyboard>/s", type: InputActionType.Button, interactions: "press");
        pauseAction.Enable(); // 입력 액션 활성화
        pauseAction.performed += context => ToggleVideoPlayback(); // 액션이 수행될 때 영상 재생/일시 정지 함수 호출
    }

    private void OnDisable()
    {
        pauseAction.Disable(); // 입력 액션 비활성화
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
}
