using UnityEngine;

namespace ExpressoBits.Inventories
{
    /// <summary>
    /// Category of an item, commonly used to categorize crafts
    /// </summary>
    [CreateAssetMenu(fileName = "Category", menuName = "Expresso Bits/Inventories/Category")]
    public class Category : ScriptableObject
    {
        public Color Color => color;
        public Sprite Icon => icon;

        [SerializeField] private Color color = Color.white;
        [SerializeField] private Sprite icon;
    }
}
