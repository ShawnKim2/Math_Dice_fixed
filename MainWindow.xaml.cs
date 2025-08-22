using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Math_Dice_fixed
{
    public partial class MainWindow : Window
    {
        private int target;
        private int[] dice;
        private int turn = 1;
        private bool gameOver = false;
        private Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
            StartGame();
        }

        private void StartGame()
        {
            target = rand.Next(1, 13) * rand.Next(1, 13);
            dice = new int[4] { rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7), rand.Next(1, 7) };
            turn = 1;
            gameOver = false;

            TargetText.Text = $"🎯 목표 숫자: {target}";
            DiceText.Text = $"🎲 주사위: {string.Join(", ", dice)}";
            LogText.Text = "=== Math Dice 게임 시작! ===\n";
            InputBox.Text = "";
            ValidationText.Text = "";
            ValidationText.Foreground = Brushes.Black;
        }

        // 실시간 숫자 검증
        private void InputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (gameOver) return;

            string expr = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(expr))
            {
                ValidationText.Text = "";
                return;
            }

            var matches = Regex.Matches(expr, @"\d+");
            int[] usedNumbers = matches.Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();
            int[] diceCopy = (int[])dice.Clone();
            bool valid = true;

            foreach (int num in usedNumbers)
            {
                int index = Array.IndexOf(diceCopy, num);
                if (index == -1)
                {
                    valid = false;
                    break;
                }
                else
                {
                    diceCopy[index] = -1; // 사용한 주사위 제거
                }
            }

            if (valid)
            {
                ValidationText.Text = "✅ 사용 가능한 주사위 숫자만 사용 중입니다.";
                ValidationText.Foreground = Brushes.Green;
            }
            else
            {
                ValidationText.Text = "❌ 주사위 숫자를 잘못 사용했습니다!";
                ValidationText.Foreground = Brushes.Red;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver) return;

            string expr = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(expr))
            {
                AppendLog("⚠️ 수식을 입력하세요.");
                return;
            }

            AppendLog($"\n--- {turn} 턴 ---");
            AppendLog($"입력한 수식: {expr}");

            // 숫자 사용 체크
            var matches = Regex.Matches(expr, @"\d+");
            int[] usedNumbers = matches.Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();
            int[] diceCopy = (int[])dice.Clone();
            bool valid = true;

            foreach (int num in usedNumbers)
            {
                int index = Array.IndexOf(diceCopy, num);
                if (index == -1)
                {
                    valid = false;
                    break;
                }
                else
                {
                    diceCopy[index] = -1;
                }
            }

            if (!valid)
            {
                AppendLog("❌ 주사위 숫자를 잘못 사용했습니다!");
                EndGame(false);
                return;
            }

            // 수식 계산
            try
            {
                var dt = new DataTable();
                var value = Convert.ToInt32(dt.Compute(expr, ""));
                AppendLog($"계산 결과: {value}");

                if (value == target)
                {
                    AppendLog("✅ 정답! 승리했습니다!");
                    EndGame(true);
                }
                else
                {
                    AppendLog($"❌ 오답! 목표({target})와 다릅니다.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"⚠️ 잘못된 수식: {ex.Message}");
            }

            turn++;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void AppendLog(string message)
        {
            LogText.Text += message + "\n";
        }

        private void EndGame(bool win)
        {
            gameOver = true;
            AppendLog("\n=== 게임 종료 ===");
            if (win)
                AppendLog("🎉 당신이 이겼습니다!");
            else
                AppendLog("💀 패배했습니다.");
        }
    }
}
