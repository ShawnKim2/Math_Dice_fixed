using System;
using System.Data;
using System.Linq;
using System.Windows;

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

            // 숫자 사용 검사
            var usedDigits = expr.Where(char.IsDigit).Select(c => int.Parse(c.ToString())).ToList();
            foreach (int d in usedDigits)
            {
                if (!dice.Contains(d))
                {
                    AppendLog($"❌ {d} 는 주사위에 없는 숫자입니다!");
                    EndGame(false);
                    return;
                }
            }

            try
            {
                // 수식 계산
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
