using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gamer.Controls;
using System.Collections.Generic;
using gamer.Entity;
using System;
using System.Xml;
using System.IO;
using System.Linq;

namespace gamer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<Button> _upgradeButtons;
        private List<TextField> _characterDescriptions;
        private List<Character> _characters;
        private int _level = 1;
        private TextField _levelText;
        private Enemy _enemy;
        private int _money = 0;
        private int _moneyMultiplier = 1;
        private TextField _moneyText;
        private int _mcDamage = 2;
        private TextField _mcDamageText;
        private TextField _idleDamageText;
        private Button _mcUpgradeButton;
        private int _mcUpgradeCost = 1;
        private int _mcLevel = 1;
        private float _characterDamageTimer;
        private Button _nextLevelButton;
        private bool _isLoop = false;
        private Button _exitButton;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900,
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var buttonTexture = Content.Load<Texture2D>("button");
            var enemyTextures = new List<Texture2D>()
            {
                Content.Load<Texture2D>("pyroslime"),
                Content.Load<Texture2D>("hydroslime"),
                Content.Load<Texture2D>("hilichurl"),
                Content.Load<Texture2D>("treasurehunter"),
                Content.Load<Texture2D>("primovishap"),
                Content.Load<Texture2D>("vishap"),
                Content.Load<Texture2D>("grader"),
                Content.Load<Texture2D>("wolf"),
                Content.Load<Texture2D>("maga"),
                Content.Load<Texture2D>("triangle")
            };

            var font72 = Content.Load<SpriteFont>("disket72");
            var font48 = Content.Load<SpriteFont>("disket48");
            var font24 = Content.Load<SpriteFont>("disket24");
            var font18 = Content.Load<SpriteFont>("disket18");
            var font14 = Content.Load<SpriteFont>("disket14");

            _characters = new List<Character>()
            {
                new Character(0, Content.Load<Texture2D>("diluc"))
                {
                    Position = new Vector2(36, 407)
                },
                new Character(1, Content.Load<Texture2D>("diona"))
                {
                    Position = new Vector2(36, 526)
                },
                new Character(2, Content.Load<Texture2D>("zhongli"))
                {
                    Position = new Vector2(36, 645)
                },
                new Character(3, Content.Load<Texture2D>("hutao"))
                {
                Position = new Vector2(36, 764)
                }
            };
            _characters.ForEach(c => c.Damage += CharacterDamage);
            _characterDescriptions = new List<TextField>
            {
                new TextField(font18, "main character\nincrease click\ndamage")
                {
                    Position = new Vector2(149, 302)
                },
                new TextField(font18, "diluc\nincrease idle\ndamage on 1")
                {
                    Position = new Vector2(149, 421)
                },
                new TextField(font18, "diona\nincrease idle\ndamage on 10")
                {
                    Position = new Vector2(149, 540)
                },
                new TextField(font18, "zhongli\nincrease idle\ndamage on 100")
                {
                    Position = new Vector2(149, 659)
                },
                new TextField(font18, "hutao\nincrease idle\ndamage on 1000")
                {
                    Position = new Vector2(149, 778)
                }
            };

            if (File.Exists("save.xml"))
                Load();

            _nextLevelButton = new Button(buttonTexture, font14, -2)
            {
                Text = "next\nlevel",
                Position = new Vector2(-1000, 0)
            };
            _nextLevelButton.Click += IncreaseLevel;

            _mcUpgradeButton = new Button(buttonTexture, font14, -1)
            {
                Position = new Vector2(515, 286),
                Text = "UPGRADE"
            };
            _mcUpgradeButton.Click += IncreaseMCDamage;

            _enemy = new Enemy(_level, font72, font48, enemyTextures, _mcDamage)
            {
                Position = new Vector2(826, 200)
            };
            _enemy.Die += IncreaseMoney;
            _enemy.BossNotKilled += DecreaseLevel;

            _upgradeButtons = new List<Button>()
            {
                new Button(buttonTexture, font14, 0)
                {
                    Position = new Vector2(515, 405),
                    Text = "UPGRADE"
                },
                new Button(buttonTexture, font14, 1)
                {
                    Position = new Vector2(515, 524),
                    Text = "UPGRADE"
                },
                new Button(buttonTexture, font14, 2)
                {
                    Position = new Vector2(515, 643),
                    Text = "UPGRADE"
                },
                new Button(buttonTexture, font14, 3)
                {
                    Position = new Vector2(515, 762),
                    Text = "UPGRADE"
                }
            };
            _upgradeButtons.ForEach(c => c.Click += UpgradeCharacter);

            _exitButton = new Button(Content.Load<Texture2D>("save"), font14, -2)
            {
                Position = new Vector2(1480, 810),
                Text = "Save and",
                Text2 = "exit"
            };
            _exitButton.Click += SaveAndExit;

            _moneyText = new TextField(font72, _money.ToString())
            {
                Position = new Vector2(40, 26),
            };
            _mcDamageText = new TextField(font24, "click damage: " + _mcDamage.ToString())
            {
                Position = new Vector2(40, 160)
            };
            _idleDamageText = new TextField(font24, "idle damage: " + GetIdleDamage(_characters))
            {
                Position = new Vector2(40, 210)
            };
            _levelText = new TextField(font48, "level " + _level.ToString())
            {
                Position = new Vector2(674, 830)
            };
        }


        private void IncreaseLevel(object sender, EventArgs e)
        {
            _level += 1;
            _moneyMultiplier += 1;
            _nextLevelButton.Position = new Vector2(-1000, 0);
            _isLoop = false;
        }

        private void DecreaseLevel(object sender, EventArgs e)
        {
            _level -= 1;
            _moneyMultiplier -= 1;
            _nextLevelButton.Position = new Vector2(1500, 0);
            _isLoop = true;
        }

        private void IncreaseMoney(object sender, EventArgs e)
        {
            int enemyMultiplier = 1;
            if (_level % 10 == 0)
                enemyMultiplier = 4;
            else if (_level % 10 == 5)
                enemyMultiplier = 2;
            _money += _moneyMultiplier * enemyMultiplier;
            if (!_isLoop)
                _level += 1;
            _moneyMultiplier = _level;
        }

        private void IncreaseMCDamage(object sender, EventArgs e)
        {
            if (_money >= _mcUpgradeCost)
            {
                _mcDamage += 2;
                _money -= _mcUpgradeCost;
                _mcUpgradeCost = (int)(1.07f * _mcUpgradeCost) + 1;
                _mcLevel += 1;
            }
        }

        private void UpgradeCharacter(object sender, EventArgs e)
        {
            var id = ((Button)sender).ID;
            if (_money >= _characters[id].UpgradeCost)
            {
                _characters[((Button)sender).ID].LevelUp();
                _money -= _characters[id].UpgradeCost-1;
            }
        }

        private void CharacterDamage(object sender, EventArgs e)
        {
            _enemy.TakeDamage(((Character)sender).characterDamage);
        }

        private int GetIdleDamage(List<Character> characters)
        {
            int result = 0;
            foreach (Character character in characters)
                result += character.characterDamage;
            return result;
        }

        private string GetNumber(int number)
        {
            var result = number.ToString();
            switch (number)
            {
                case >= 1000000000:
                    result = Math.Round((number / 1000000000f), 1).ToString() + "b";
                    break;
                case >= 1000000:
                    result = Math.Round((number / 1000000f), 1).ToString() + "m";
                    break;
                case >= 1000:
                    result = Math.Round((number / 1000f), 1).ToString() + "k";
                    break;
            }
            return result;
        }

        private void SaveAndExit(object sender, EventArgs e)
        {
            var writer = XmlWriter.Create("save.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("mc");
            writer.WriteAttributeString("_mcDamage", _mcDamage.ToString());
            writer.WriteAttributeString("_mcUpgradeCost", _mcUpgradeCost.ToString());
            writer.WriteAttributeString("_money", _money.ToString());
            writer.WriteAttributeString("_level", _level.ToString());
            foreach (var character in _characters)
                character.Save(writer);
            writer.WriteEndDocument();
            writer.Close();
            Exit();
        }

        private void Load()
        {
            var reader = XmlReader.Create("save.xml");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    switch (reader.Name)
                    {
                        case "mc":
                            _mcDamage = int.Parse(reader.GetAttribute("_mcDamage"));
                            _mcUpgradeCost = int.Parse(reader.GetAttribute("_mcUpgradeCost"));
                            _money = int.Parse(reader.GetAttribute("_money"));
                            _level = int.Parse(reader.GetAttribute("_level"));
                            break;
                        case "ID0":
                            _characters[0].LoadSave(reader); 
                            break;
                        case "ID1":
                            _characters[1].LoadSave(reader);
                            break;
                        case "ID2":
                            _characters[2].LoadSave(reader);
                            break;
                        case "ID3":
                            _characters[3].LoadSave(reader);
                            break;
                    }
            }
            reader.Close();
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var button in _upgradeButtons)
                button.Update(gameTime, GetNumber(_characters[button.ID].UpgradeCost));
            _mcUpgradeButton.Update(gameTime, GetNumber(_mcUpgradeCost));

            _nextLevelButton.Update(gameTime, null);

            foreach (var description in _characterDescriptions)
                description.Update(gameTime, null);

            _moneyText.Update(gameTime, GetNumber(_money));
            _mcDamageText.Update(gameTime, "click damage: " + GetNumber(_mcDamage));
            _idleDamageText.Update(gameTime, "idle damage: " + GetNumber(GetIdleDamage(_characters)));
            _levelText.Update(gameTime, "level " + GetNumber(_level));

            if (_characterDamageTimer < 1)
                _characterDamageTimer += 1.0f / 60;
            else
            {
                _characterDamageTimer = 0;
                foreach (var character in _characters)
                    character.Update(gameTime);
            }

            _enemy.Update(gameTime, _mcDamage, _isLoop);
            _exitButton.Update(gameTime, null);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _spriteBatch.Draw(Content.Load<Texture2D>("fone"), new Vector2(0, 0), Color.White);
            _nextLevelButton.Draw(gameTime, _spriteBatch);
            _spriteBatch.Draw(Content.Load<Texture2D>("mc"), new Vector2(36, 288), Color.White);

            foreach (var character in _characters)
                character.Draw(gameTime, _spriteBatch);

            foreach (var button in _upgradeButtons)
                button.Draw(gameTime, _spriteBatch);
            _mcUpgradeButton.Draw(gameTime, _spriteBatch);

            foreach (var text in _characterDescriptions)
                text.Draw(gameTime, _spriteBatch);

            _moneyText.Draw(gameTime, _spriteBatch);
            _mcDamageText.Draw(gameTime, _spriteBatch);
            _idleDamageText.Draw(gameTime, _spriteBatch);
            _levelText.Draw(gameTime, _spriteBatch);

            _enemy.Draw(gameTime, _spriteBatch);
            _exitButton.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}