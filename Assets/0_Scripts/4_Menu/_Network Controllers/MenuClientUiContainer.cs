using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Badbarbos.Menu
{
    public class MenuClientUiContainer : MonoBehaviour
    {
        #region Choose UI

        [Header("Choose UI")]

        public Button CHOOSE_CreateMenuButton;

        public Button CHOOSE_JoinMenuButton;

        public TMP_InputField CHOOSE_CodeForFriendField;

        public Button CHOOSE_ReturnButton;

        #endregion

        #region Create UI

        [Header("Create UI")]

        public TextMeshProUGUI CREATE_CodeForFriendText;

        public TextMeshProUGUI CREATE_PlayerListText;

        public Button CREATE_StartButton;

        public Button CREATE_ReturnButton;

        #endregion

        #region Join UI

        [Header("Join UI")]

        public TextMeshProUGUI JOIN_CodeForFriendText;

        public TextMeshProUGUI JOIN_PlayerListText;

        public Button JOIN_ReturnButton;

        #endregion
    }
}
