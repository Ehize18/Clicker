using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace gamer
{
    public class Character
    {
        private int ID;
        private List<int> _damageMultipliers = new List<int>
        {
            1, 10, 100, 1000
        };
        private List<int> _firstUpgradeCosts = new List<int>
        {
            10, 100, 1000, 10000
        };
        public int UpgradeCost;
        private float _upgradeCostMultiplier = 1.07f;
        private bool IsUnlocked = false;
        private int _level = 0;
        public int characterDamage = 0;
        private Texture2D _texture;
        public EventHandler Damage;
        public Vector2 Position { get; set; }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }
        public Character(int id, Texture2D texture)
        {
            ID = id;
            _texture = texture;
            UpgradeCost = _firstUpgradeCosts[ID];
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsUnlocked)
                spriteBatch.Draw(_texture, Rectangle, Color.White);
            else
                spriteBatch.Draw(_texture, Rectangle, Color.Black);
        }

        public void Update(GameTime gameTime)
        {
            if (IsUnlocked)
                Damage?.Invoke(this, new EventArgs());
        }

        public void LevelUp()
        {
            if (!IsUnlocked)
                IsUnlocked = true;
            _level += 1;
            characterDamage = _level*_damageMultipliers[ID];
            UpgradeCost = (int)(_upgradeCostMultiplier*UpgradeCost) + 1;
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("ID" + ID.ToString());
            writer.WriteAttributeString("IsUnlocked", IsUnlocked.ToString());
            writer.WriteAttributeString("_level", _level.ToString());
            writer.WriteAttributeString("characterDamage", characterDamage.ToString());
            writer.WriteAttributeString("UpgradeCost", UpgradeCost.ToString());
        }

        public void LoadSave(XmlReader reader)
        {
            var gettedIsUnlocked = reader.GetAttribute("IsUnlocked");
            if (gettedIsUnlocked == "True")
                IsUnlocked = true;
            var temp = int.Parse(reader.GetAttribute("_level"));
            _level = int.Parse(reader.GetAttribute("_level"));
            characterDamage = int.Parse(reader.GetAttribute("characterDamage"));
            UpgradeCost = int.Parse(reader.GetAttribute("UpgradeCost"));
        }
    }
}
