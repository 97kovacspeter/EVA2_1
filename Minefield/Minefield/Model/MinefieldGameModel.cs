using System;
using Minefield.Persistence;
using System.Threading.Tasks;

namespace Minefield.Model
{
    #region enums

    /// <summary>
    /// Bombák nehézségi felsorolása.
    /// </summary>
    public enum Weight { Light, Medium, Heavy }

    /// <summary>
    /// Irányok felsorolása.
    /// </summary>
    public enum Dir { Left,Right,Up, Down }

    #endregion

    /// <summary>
    /// Aknamező játék típusa.
    /// </summary>
    public class MinefieldGameModel
    {
        #region Variables

        private MinefieldTable _table;
        private IMinefieldDataAccess _dataAccess;
        private Int32 _gameTime;
        private bool _dead=false;
        private bool _pause = false;
        private int _playerX = 9;
        private int _playerY = 9;
        Random rnd = new Random();

        #endregion

        #region Constructor

        /// <summary>
        /// Aknamező játék példányosítása.
        /// </summary>
        public MinefieldGameModel(IMinefieldDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _table = new MinefieldTable();
            _dead = false;
        }


        #endregion

        #region Events

        /// <summary>
        /// Játék előrehaladásának eseménye.
        /// </summary>
        public event EventHandler GameAdvanced;

        /// <summary>
        /// Játék végének eseménye.
        /// </summary>
        public event EventHandler GameOver;

        #endregion

        #region Properties

        /// <summary>
        /// Eltelt játékidő lekérdezése.
        /// </summary>
        public Int32 GameTime { get { return _gameTime; } }


        /// <summary>
        /// Játéktábla lekérdezése.
        /// </summary>
        public MinefieldTable Table { get { return _table; } }

        /// <summary>
        /// Játék végének lekérdezése.
        /// </summary>
        public bool IsGameOver { get { return _dead; } }

        #endregion

        #region Public game methods

        #region AdvanceBomb

        /// <summary>
        /// Bombák léptetése
        /// </summary>
        /// <param name="weight"></param>
        public void AdvanceBomb(Weight weight)
        {
            FieldType tempField = FieldType.Empty;
            switch (weight)
            {
                case Weight.Light:
                    tempField = FieldType.LightB;
                    break;
                case Weight.Medium:
                    tempField = FieldType.MediumB;
                    break;
                case Weight.Heavy:
                    tempField = FieldType.HeavyB;
                    break;
            }           


            for (int i = 9; i>=0; i--)
            {
                for (int j = 9; j >=0; j--)
                {
                    if (_table.fieldValues[i, j] == tempField && i<9)
                    {
                        _table.fieldValues[i, j] = FieldType.Empty;
                        if (_table.fieldValues[i+1,j]==FieldType.Player)
                        {
                            _dead = true;
                            if (GameAdvanced != null) {
                                GameAdvanced(this, null);
                            }
                            _table.fieldValues[i + 1, j] = tempField;
                            if (GameOver != null)
                            {
                                GameOver(this, null);
                            }
                            return;
                        }
                        _table.fieldValues[i + 1, j] = tempField;
                    }
                    if( _table.fieldValues[i, j]==tempField&& i==9)
                    {
                        _table.fieldValues[i, j] = FieldType.Empty;
                    }
                   
                }
            }
        }

        #endregion

        #region NewGame

        /// <summary>
        /// Új játék kezdése.
        /// </summary>
        public void NewGame()
        {
            _table = new MinefieldTable();
            _playerX = 9;
            _playerY = 9;
            _dead = false;
            _gameTime = 0;
        }

        #endregion
        
        #region AdvanceTime

        /// <summary>
        /// Játékidő léptetése.
        /// </summary>
        public void AdvanceTime()
        {
            if (IsGameOver) 
                return;

            if (_gameTime % 3 == 0)
            {
                AdvanceBomb(Weight.Light);
            }
            if (_gameTime % 2 == 0)
            {
                AdvanceBomb(Weight.Medium);
            }
            AdvanceBomb(Weight.Heavy);

            _gameTime++;
            
            if (_gameTime % 2 == 0)
            {
                Generator();
                if (_gameTime > 5)
                {
                    Generator();
                }
                if (_gameTime > 7)
                {
                    Generator();
                }
            }
            if (GameAdvanced != null)
            {
                GameAdvanced(this, null);
            }
        }

        #endregion

        #region Step

        /// <summary>
        /// Játékos léptetése
        /// </summary>
        /// <param name="direction"></param>
        public void Step(Dir direction)
        {

            if (IsGameOver)
                return;

            if (_pause)
                return;


            if(direction==Dir.Right)
            {
                if(_playerY<9)
                {
                    _table.fieldValues[_playerX, _playerY] = FieldType.Empty;
                    if(_table.fieldValues[_playerX, _playerY+1] != FieldType.Empty)
                    {
                        _dead = true;
                        if (GameAdvanced != null)
                        {
                            GameAdvanced(this, null);
                        }
                        if (GameOver != null)
                        {
                            GameOver(this, null);
                        }
                        return;
                    }
                    _table.fieldValues[_playerX, _playerY+1] = FieldType.Player;
                    _playerY++;
                }
            }
            if (direction == Dir.Left)
            {
                if (_playerY > 0)
                {
                    _table.fieldValues[_playerX, _playerY] = FieldType.Empty;
                    if (_table.fieldValues[_playerX, _playerY-1] != FieldType.Empty)
                    {
                        _dead = true;
                        if (GameAdvanced != null)
                        {
                            GameAdvanced(this, null);
                        }
                        if (GameOver != null)
                        {
                            GameOver(this, null);
                        }
                        return;
                    }
                    _table.fieldValues[_playerX, _playerY - 1] = FieldType.Player;
                    _playerY--;
                }

            }
            if (direction == Dir.Up)
            {
                if (_playerX >0)
                {
                    _table.fieldValues[_playerX, _playerY] = FieldType.Empty;
                    if (_table.fieldValues[_playerX-1, _playerY] != FieldType.Empty)
                    {
                        _dead = true;
                        if (GameAdvanced != null)
                        {
                            GameAdvanced(this, null);
                        }
                        if (GameOver != null)
                        {
                            GameOver(this, null);
                        }
                        return;
                    }
                    _table.fieldValues[_playerX-1, _playerY] = FieldType.Player;
                    _playerX--;
                }
            }
            if (direction == Dir.Down)
            {
                if (_playerX < 9)
                {
                    _table.fieldValues[_playerX, _playerY] = FieldType.Empty;
                    if (_table.fieldValues[_playerX+1, _playerY] != FieldType.Empty)
                    {
                        _dead = true;
                        if (GameAdvanced != null)
                        {
                            GameAdvanced(this, null);
                        }
                        if (GameOver != null)
                        {
                            GameOver(this, null);
                        }
                        return;
                    }
                    _table.fieldValues[_playerX+1, _playerY] = FieldType.Player;
                    _playerX++;
                }
            }


            if (GameAdvanced != null)
            {
                GameAdvanced(this, null);
            }
        }

        #endregion

        #region Pause

        public void Pause()
        {
            _pause = true;
        }

        #endregion

        #region Continue

        public void Continue()
        {
            _pause = false;
        }

        #endregion

        #endregion

        #region Async

        #region Load

        /// <summary>
        /// Játék betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");
            
            ReturnData returndata = await _dataAccess.LoadAsync(path);

            _gameTime = returndata.gameTime;
            _table = returndata.table;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if(_table.fieldValues[i, j] == FieldType.Player)
                    {
                        _playerX = i;
                        _playerY = j;
                    }
                }
            }
            
        }

        #endregion

        #region Save

        /// <summary>
        /// Játék mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        public async Task SaveGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, _table,_gameTime);
        }

        #endregion

        #endregion

        #region Private game methods

        #region Generating bombs

        private void Generator()
        {
            
            int column = rnd.Next(0,10);

            //Random rnd2 = new Random();
            int type = rnd.Next(0,3);

            switch (type)
            {
                case 0:
                    _table.fieldValues[0, column] = FieldType.LightB;
                    break;
                case 1:
                    _table.fieldValues[0, column] = FieldType.MediumB;
                    break;
                case 2:
                    _table.fieldValues[0, column] = FieldType.HeavyB;
                    break;
            }   
        }

        #endregion

        #endregion
    }
}
