using UnityEngine;

namespace Managers.Avatar
{
    public class AvatarSpriteMaskingController : MonoBehaviour
    {
        private SpriteMask mask;
        [SerializeField] private Load2DSpriteRenderer avatarSpriteLoader;
        [SerializeField] private Sprite defaultSprite;

        void Start()
        {
            mask = GetComponent<SpriteMask>();
            if (mask!=null)
            {
                mask.sprite = defaultSprite;
            }
        }

        private void OnEnable()
        {
            avatarSpriteLoader.OnLoadingImageComplete += SetMaskSpriteFromSource;
        }

        private void OnDisable()
        {
            avatarSpriteLoader.OnLoadingImageComplete -= SetMaskSpriteFromSource;
        }

        void SetMaskSpriteFromSource(bool flag, Sprite sprite)
        {
            if (!flag) return;
            if (mask != null)
            {
                mask.sprite = sprite;
            }
        }
    }
}