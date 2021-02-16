using System;
using System.Windows.Forms;
using Minefield.Model;
using Minefield.Persistence;
using System.Drawing;


namespace Minefield
{
    /// <summary>
    /// Játékablak típusa.
    /// </summary>
    public partial class GameForm : Form
    {
        
        #region Fields

        private IMinefieldDataAccess _dataAccess;
        private MinefieldGameModel _model;
        private Button[,] _game_buttons;
        private Timer _timer;

        #endregion

        #region Constructors

        /// <summary>
        /// Játékablak példányosítása.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Eventek és tábla generálás
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameForm_Load(Object sender, EventArgs e)
        {
            _dataAccess = new MinefieldFileDataAccess();
            _model = new MinefieldGameModel(_dataAccess);
            GenerateTable();

            // adatelérés példányosítása

            // _dataAccess = new MinefieldFileDataAccess();
            // időzítő létrehozása
            _timer = new Timer
            {
                Interval = 250
            };
            label1.Text = "TIMER: 0";
            _timer.Tick += new EventHandler(timer1_Tick);
            KeyPreview = true;
            KeyDown += new KeyEventHandler(Key_Down);
            
            viewUpdate(this,null);
            _model.GameAdvanced += new EventHandler(viewUpdate);
            _model.GameOver += new EventHandler(gameOver);
            _timer.Start();
            pictureBox1.BackgroundImage = Properties.Resources.csighajo;

        }

        #endregion

        #region Key events

        /// <summary>
        /// Irányítás
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Key_Down(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.D)
            {
                _model.Step(Dir.Right);
            }
            else if(e.KeyCode == Keys.S)
            {
                _model.Step(Dir.Down);
            }
            else if (e.KeyCode == Keys.A)
            {
                _model.Step(Dir.Left);
            }
            else if (e.KeyCode == Keys.W)
            {
                _model.Step(Dir.Up);
            }
        }
        #endregion

        #region Table generating

        /// <summary>
        /// Tábla generálás
        /// </summary>
        private void GenerateTable()
        {
            _game_buttons = new Button[10, 10];
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    _game_buttons[i, j] = new Button();
                    _game_buttons[i, j].Location = new Point(5 + 50 * j, 147 + 50 * i);
                    _game_buttons[i, j].Size = new Size(50, 50);
                    _game_buttons[i, j].TabIndex = 100 + i * 10 + j;
                   
                    Controls.Add(_game_buttons[i, j]);
                }
            }


        }

        #endregion
        
        #region Timer event handler

        private void timer1_Tick(object sender, EventArgs e)
        {
            _model.AdvanceTime(); // játék léptetése
        }

        #endregion

        #region Update

        public void viewUpdate(object sender, EventArgs e)
        {
            label1.Text ="TIMER: " + _model.GameTime.ToString();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {

                    if (_model.Table.fieldValues[i, j] == FieldType.Player)
                    {
                        _game_buttons[i, j].BackgroundImage = Properties.Resources.hajo;
                    }
                    else if (_model.Table.fieldValues[i, j] == FieldType.LightB)
                    {
                        _game_buttons[i, j].BackgroundImage = Properties.Resources.mineL;
                    }
                    else if (_model.Table.fieldValues[i, j] == FieldType.MediumB)
                    {
                        _game_buttons[i, j].BackgroundImage = Properties.Resources.mineM;
                    }
                    else if (_model.Table.fieldValues[i, j] == FieldType.HeavyB)
                    {
                        _game_buttons[i, j].BackgroundImage = Properties.Resources.mineH;
                    }
                    else if (_model.Table.fieldValues[i, j] == FieldType.Empty)
                    {
                        _game_buttons[i, j].BackColor = Color.White;
                        _game_buttons[i, j].BackgroundImage = null;
                    }

                }
            }

        }

        #endregion

        #region GameOver

        public void gameOver(object sender, EventArgs e)
        {
            
            if(MessageBox.Show("NEW GAME", "GAME OVER", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _model.NewGame();
            }
            else
            {
                _model.Pause();
            }


        }

        #endregion

        #region Menu events

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _model.NewGame();
        }

        private void pauseToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            _timer.Stop();
            _model.Pause();
            
            pauseToolStripMenuItem.Enabled = false;         
            continueToolStripMenuItem.Enabled = true;
            loadToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
        }

        private void continueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Start();
            _model.Continue();
            pauseToolStripMenuItem.Enabled = true;
            continueToolStripMenuItem.Enabled = false;
            loadToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
        }

        private async void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Stop();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _model.LoadGameAsync(openFileDialog1.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Játék betöltése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a fájlformátum.", "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _model.NewGame();
                }
            }

            _model.Pause();
            viewUpdate(this, null);
        }

        private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Stop();

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    
                    await _model.SaveGameAsync(saveFileDialog1.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            _model.Pause();
        }

       


        #endregion

        #region Pic unrelated

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
