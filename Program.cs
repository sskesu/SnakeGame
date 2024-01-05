using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Linq;


class Program
{
    //    맵 만들기
    public static int mapX = 20, mapY = 20;

    public static void mapDraw()
    {
        for (int i = 0; i <= mapX; i++)
        {
            for (int j = 0; j <= mapY; j++)
            {
                if (i == 0 || i == mapX || j == 0 || j == mapY)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write("■");
                }
            }
        }
    }

    // 음식 만들기
    class FoodCreator
    {
        int x;
        int y;
        char c;
        Random rand = new Random();

        public FoodCreator(int _x, int _y, char _c)
        {
            x = _x;
            y = _y;
            c = _c;
        }

        public Point CreateFood()
        {
            Point p = new Point(rand.Next(1, y), rand.Next(1, y), c);
            return p;
        }
    }

    // 메인 함수
    static void Main(string[] args)
    {
        // 뱀의 초기 위치와 방향을 설정하고, 그립니다.
        Point p = new Point(4, 5, '*');
        Snake snake = new Snake(p, 4, Direction.RIGHT);
        snake.DrawSnake();

        // 음식의 위치를 무작위로 생성하고, 그립니다.
        FoodCreator foodCreator = new FoodCreator(mapX, mapY, '$');
        Point food = foodCreator.CreateFood();
        food.Draw();

        mapDraw();
        ConsoleKeyInfo cki;
        // 게임 루프: 이 루프는 게임이 끝날 때까지 계속 실행됩니다.
        while (true)
        {
            // 키 입력이 있는 경우에만 방향을 변경합니다.
            if (Console.KeyAvailable)
            {
                cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (snake.dir != Direction.DOWN)
                        {
                            snake.dir = Direction.UP;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (snake.dir != Direction.UP)
                        {
                            snake.dir = Direction.DOWN;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (snake.dir != Direction.LEFT)
                        {
                            snake.dir = Direction.RIGHT;
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (snake.dir != Direction.RIGHT)
                        {
                            snake.dir = Direction.LEFT;
                        }
                        break;
                }
            }

            // 뱀이 이동하고, 음식을 먹었는지, 벽이나 자신의 몸에 부딪혔는지 등을 확인하고 처리하는 로직을 작성하세요.
            // 이동, 음식 먹기, 충돌 처리 등의 로직을 완성하세요.
            if (snake.IsHitWall())
            {
                break;
            }
            if (snake.IsHitSnake())
            {
                break;
            }
            // 음식 먹었는지
            if (snake.IsEatFood(food))
            {
                snake.length++;
                food = foodCreator.CreateFood();
                food.Draw();
            }
            snake.Move(snake.length);

            Thread.Sleep(100); // 게임 속도 조절 (이 값을 변경하면 게임의 속도가 바뀝니다)

            // 뱀의 상태를 출력합니다 (예: 현재 길이, 먹은 음식의 수 등)
            Console.SetCursorPosition(0, 21);
            Console.WriteLine($"현재 길이 : {snake.length}, 먹은 음식의 수 : {snake.length - 4}");

        }
        Console.WriteLine("Game Over");
    }
}

// 뱀
public class Snake
{
    public Point p;
    public Direction dir;
    public int length;
    List<Point> snakePoints = new List<Point>();


    public Snake(Point _p, int _length, Direction _dir)
    {

        dir = _dir;
        length = _length;
        for (int i = 0; i < _length; i++)
        {
            Point p = new Point(_p.x, _p.y, _p.sym);
            snakePoints.Add(p);
            _p.x++;
        }
    }
    // 뱀 그리기
    public void DrawSnake()
    {
        foreach (Point item in snakePoints)
        {
            item.Draw();
        }
    }
    // 이동
    public void Move(int _length)
    {
        // 음식 먹었으면 꼬리 안자름
        if (snakePoints.Count != _length)
        {
            snakePoints.First().Clear();
            snakePoints.Remove(snakePoints.First());
        }
        Point p = GetNextPoint();
        snakePoints.Add(p);
        p.Draw();
    }
    public Point GetNextPoint()
    {
        Point head = snakePoints.Last();
        Point nextPoint = new Point(head.x, head.y, head.sym);
        switch (dir)
        {
            case Direction.LEFT:
                nextPoint.x -= 1;
                break;
            case Direction.RIGHT:
                nextPoint.x += 1;
                break;
            case Direction.UP:
                nextPoint.y -= 1;
                break;
            case Direction.DOWN:
                nextPoint.y += 1;
                break;
        }
        return nextPoint;
    }
    // 벽에 부딪혔는지
    public bool IsHitWall()
    {
        if (snakePoints.Last().x <= 0 || snakePoints.Last().y == 0 || snakePoints.Last().x == Program.mapX || snakePoints.Last().y == Program.mapY)
        {
            return true;
        }
        return false;
    }
    // 자기 몸에 부딪혔는지
    public bool IsHitSnake()
    {
        for (int i = 0; i < snakePoints.Count - 1; i++)
        {
            if (snakePoints.Last().IsHit(snakePoints[i]))
            {
                return true;
            }
        }
        return false;
    }
    // 음식 먹었는지
    public bool IsEatFood(Point _food)
    {
        if (_food.IsHit(snakePoints.Last()))
        {
            return true;
        }
        return false;
    }
}

public class Point
{
    public int x { get; set; }
    public int y { get; set; }
    public char sym { get; set; }


    // Point 클래스 생성자
    public Point(int _x, int _y, char _sym)
    {
        x = _x;
        y = _y;
        sym = _sym;
    }

    // 점을 그리는 메서드
    public void Draw()
    {
        Console.SetCursorPosition(x, y);
        Console.Write(sym);
    }


    // 점을 지우는 메서드
    public void Clear()
    {
        sym = ' ';
        Draw();
    }

    // 두 점이 같은지 비교하는 메서드
    public bool IsHit(Point p)
    {
        return p.x == x && p.y == y;
    }
}
// 방향을 표현하는 열거형입니다.
public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}