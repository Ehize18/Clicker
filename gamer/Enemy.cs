using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gamer.Entity
{
    public class Enemy
    {
        private float _health;
        private float _maxHealth;
        public int EnemyType;
        private float _bossTimer;
        private int _mcDamage;
        private int _level;
        private bool _isLoop;
        private MouseState _currentMouse;

        private SpriteFont _font;

        private MouseState _previousMouse;

        private List<Texture2D> _textureList;

        private Texture2D _texture;

        public event EventHandler Die;

        public event EventHandler BossNotKilled;

        public bool Clicked { get; private set; }

        public Vector2 Position { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }
        public Enemy(int level, SpriteFont font, List<Texture2D> textureList, int mcDamage)
        {
            _mcDamage = mcDamage;
            _font = font;
            _textureList = textureList;
            SetEnemy(level);
            _level = level;
        }

        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Rectangle, Color.White);
            string hp;
            if (_health < 0)
                hp = "0/" + _maxHealth.ToString();
            else
                hp = _health.ToString() + "/" + _maxHealth.ToString();
            var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(hp).X / 2);
            var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(hp).Y / 2) + -300;
            spriteBatch.DrawString(_font, hp, new Vector2(x, y), Color.White);
            if (EnemyType == 2)
                spriteBatch.DrawString(_font, ((int)_bossTimer).ToString() + "s", new Vector2(x + 150, y - 100), Color.White);
        }

        public void Update(GameTime gameTime, int mcDamage, bool isLoop)
        {
            if (_isLoop == true && _isLoop != isLoop)
            {
                _isLoop = isLoop;
                _level += 1;
                SetEnemy(_level);
            }
            else
                _isLoop = isLoop;
            if (EnemyType == 2)
            {
                _bossTimer -= 1f / 60;
                if (_bossTimer <= 0)
                {
                    BossNotKilled?.Invoke(this, new EventArgs());
                    _level -= 1;
                    SetEnemy(_level);
                }
            }
            _mcDamage = mcDamage;
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            if (_health <= 0)
            {
                if (!isLoop)
                    _level += 1;
                SetEnemy(_level);
                Die?.Invoke(this, new EventArgs());
            }
            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);
            if (mouseRectangle.Intersects(Rectangle))
            {
                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    _health -= _mcDamage;
                }
            }
        }
        private void SetEnemy(int level)
        {
            var random = new Random();
            if (level % 10 == 0)
            {
                _health = 10 * (int)(level * level * 0.36 + 10);
                _maxHealth = _health;
                _bossTimer = 30;
                EnemyType = 2;
                _texture = _textureList[random.Next(7, 10)];
            }
            else if (level % 10 == 5)
            {
                _health = 5 * (int)(level * level * 0.36 + 10);
                _maxHealth = _health;
                EnemyType = 1;
                _texture = _textureList[random.Next(4, 7)];
            }
            else
            {
                _health = (int)(level * level * 0.36 + 10);
                _maxHealth = _health;
                EnemyType = 0;
                _texture = _textureList[random.Next(0, 4)];
            }
        }

        public void TakeDamage(int Damage)
        {
            _health -= Damage;
        }
    }
}
