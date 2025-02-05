using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading;
using static TextRPG.Program;
using System.Runtime.CompilerServices;


namespace TextRPG
{
    internal class Program
    {
        public class Player //플레이어 클래스
        {
            public string name;
            public int level;
            public string chad;
            public double attack;
            public int shield;
            public int hp;
            public int gold;
            public int clearcount;

            public List<Item> inventory = new List<Item>(); //아이템을 보유할 인벤토리
        }

        public enum ItemType
        {
            Weapon, //무기
            Armor //방어구
        }
        public class Item //아이템 클래스
        {
            public string Name { get; set; }
            public int Attack { get; set; }
            public int Shield { get; set; }
            public string Explain { get; set; }
            public int Price {  get; set; }

            public bool Equip { get; set; } 
            public ItemType Type { get; set; }

            
            public Item(string name, int attack, int shield, string explain, int price, ItemType type)
            {
                Name = name;
                Attack = attack;
                Shield = shield;
                Explain = explain;
                Price = price;
                Equip = false;
                Type = type;
            }

            // Equals 메서드 오버라이드
            public override bool Equals(object obj)
            {
                if (obj is Item otherItem)
                {
                    return this.Name == otherItem.Name && this.Price == otherItem.Price;
                }
                return false;
            }

            // GetHashCode 오버라이드
            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ Price.GetHashCode();
            }
        }
        public static void SetColor(ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
        }
        public static void ResetColor()
        {
            Console.ResetColor();
        }
        public static void LevelUp(Player player)
        {
            if (player.clearcount >= player.level)
            {
                player.level++;

                player.attack += 0.5;
                player.shield += 1;

                Console.WriteLine();
                SetColor(ConsoleColor.Blue);
                Console.WriteLine("레벨업 했습니다!");
                ResetColor();
                Thread.Sleep(2000);
                Console.WriteLine($"현재 레벨 : {player.level}");
                Console.WriteLine("공격력, 방어력이 증가했습니다!");
                Console.WriteLine($"공격력 : {player.attack}, 방어력 : {player.shield}");
                Console.WriteLine();
            }
        }
        public static void Dungeon(Player player)
        {
            Console.WriteLine("던전입장");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 쉬운 던전 | 방어력 5 이상 권장");
            Console.WriteLine("2. 일반 던전 | 방어력 11 이상 권장");
            Console.WriteLine("3. 어려운 던전 | 방어력 17 이상 권장");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요");
            Console.Write(">>");

            string input = Console.ReadLine();

            if (input == "0")
            {
                start(player);

            }
            else
            {
                int DungeonShield = 0;
                int Reward = 0;
                string dungeonName = "";

                switch (input)
                {
                    case "1":
                        DungeonShield = 5;
                        Reward = 1000;
                        dungeonName = "쉬운 던전";
                        break;
                    case "2":
                        DungeonShield = 11;
                        Reward = 1700;
                        dungeonName = "일반 던전";
                        break;
                    case "3":
                        DungeonShield = 17;
                        Reward = 2500;
                        dungeonName = "어려운 던전";
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(1000);
                        Console.Clear();
                        Dungeon(player);
                        return;

                }

                // 던전 클리어 전 상태 
                int initialHp = player.hp;
                int initialGold = player.gold;

                Random rand = new Random();

                if (player.shield < DungeonShield)
                {
                    Console.WriteLine("권장 방어력보다 부족합니다. 던전 실패 확률 40%...");
                    Console.WriteLine();
                    int randomNum = rand.Next(1, 101); 

                    // 실패 확률이 40% 아래일 경우
                    if (randomNum <= 40)
                    {
                        // 던전 실패

                        player.hp -= player.hp / 2;

                        Console.Clear();
                        Console.WriteLine("던전 실패");
                        Console.WriteLine($"{dungeonName}을 클리어 하지 못했습니다...");
                        Console.WriteLine("보상을 얻지 못하고 체력이 절반 감소합니다.");
                        Console.WriteLine();
                        Console.WriteLine("[탐험 결과]");
                        Console.WriteLine($"체력 {initialHp} -> {player.hp}");
                        Console.WriteLine($"Gold는 얻지 못 했어요 ㅠㅠ!");
                        Console.WriteLine();
                        Console.WriteLine($"3초 뒤 던전 입장 화면으로 돌아갑니다.");
                        Console.WriteLine();
                        Thread.Sleep(3000); //3초 대기
                        Dungeon(player);
                    }
                    else
                    {
                        // 체력 소모 계산 
                        int healthLoss = rand.Next(20, 36) - (player.shield - DungeonShield); // (내 방어력 - 권장 방어력) 만큼 랜덤 값에 적용
                        player.hp -= healthLoss;

                        // 공격력에 따른 보상 계산
                        int attackRewardPercentage = rand.Next((int)player.attack, (int)player.attack * 2 + 1);
                        int totalReward = Reward + (Reward * attackRewardPercentage / 100);
                        player.gold += totalReward;
                        // 클리어 횟수 계산
                        player.clearcount++;
                        
                        // 던전 클리어 후 결과 출력
                        Console.WriteLine($"방어력이 부족했지만 ... ");
                        Thread.Sleep(1000);
                        Console.WriteLine("축하합니다!!");
                        Console.WriteLine($"{dungeonName}을 클리어 하였습니다!!");
                        Console.WriteLine();
                        Console.WriteLine("[탐험 결과]");
                        Console.WriteLine($"체력 {initialHp} -> {player.hp}");
                        Console.WriteLine($"Gold {initialGold} G -> {player.gold} G");
                        Console.WriteLine($"공격력에 따른 추가 보상: {attackRewardPercentage}%");
                        Console.WriteLine($"총 보상: {totalReward} G");
                        LevelUp(player);
                        Console.WriteLine();
                        Console.WriteLine("0. 나가기");
                        Console.WriteLine("원하시는 행동을 입력해주세요.");
                        Console.Write(">>");

                        string exitInput = Console.ReadLine();
                        if (exitInput == "0")
                        {
                            Dungeon(player);
                        }
                        else
                        {
                            Console.WriteLine("잘못된 입력입니다.");
                        }
                    }
                }
                else
                {
                    // 체력 소모 계산 
                    int healthLoss = rand.Next(20, 36) - (player.shield - DungeonShield); // (내 방어력 - 권장 방어력) 만큼 랜덤 값에 적용
                    player.hp -= healthLoss;

                    // 공격력에 따른 보상 계산
                    int attackRewardPercentage = rand.Next((int)player.attack, (int)player.attack * 2 + 1);
                    int totalReward = Reward + (Reward * attackRewardPercentage / 100);
                    player.gold += totalReward;
                    player.clearcount++;

                    // 던전 클리어 후 결과 출력
                    Console.WriteLine("던전 클리어");
                    Console.WriteLine("축하합니다!!");
                    Console.WriteLine($"{dungeonName}을 클리어 하였습니다.");
                    Console.WriteLine();
                    SetColor(ConsoleColor.DarkGreen);
                    Console.WriteLine("[탐험 결과]");
                    ResetColor();
                    Console.WriteLine($"체력 {initialHp} -> {player.hp}");
                    Console.WriteLine($"Gold {initialGold} G -> {player.gold} G");
                    Console.WriteLine($"공격력에 따른 추가 보상: {attackRewardPercentage}%");
                    Console.WriteLine($"총 보상: {totalReward} G");
                    LevelUp(player);
                    Console.WriteLine();
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    SetColor(ConsoleColor.Yellow);
                    Console.Write(">>");
                    ResetColor();

                    string exitInput = Console.ReadLine();
                    if (exitInput == "0")
                    {
                        Dungeon(player);
                    }
                }

                
            }
           
        }
        public static void Rest(Player player)
        {
            SetColor(ConsoleColor.Blue);
            Console.WriteLine("휴식하기");
            ResetColor();
            Console.Write($"500G 를 내면 체력을 회복할 수 있습니다."); 
            Console.Write("보유 골드 : ");
            SetColor(ConsoleColor.Yellow);
            Console.Write($"{player.gold}");
            ResetColor();
            Console.WriteLine(" G");
            Console.WriteLine();
            Console.WriteLine("1. 휴식하기");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요");
            SetColor(ConsoleColor.Yellow);
            Console.Write(">>");
            ResetColor();

            string input = Console.ReadLine();

            if (input == "0")
            {
                start(player);

            }
            else if (input == "1")
            {
                if (player.gold >= 500)
                {
                    player.gold -= 500;
                    SetColor(ConsoleColor.Blue);
                    Console.WriteLine("휴식을 완료했습니다.");
                    ResetColor();
                    player.hp = 100;
                    Thread.Sleep(2000); // 2초 대기
                    Console.Clear();
                    Rest(player);
                }
                else
                {
                    SetColor(ConsoleColor.Red);
                    Console.WriteLine("Gold가 부족합니다.");
                    ResetColor();
                    Thread.Sleep(2000); // 2초 대기
                    Console.Clear();
                    Rest(player);
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Thread.Sleep(1000); // 1초 대기
                Console.Clear();
                Rest(player);
            }


        }
        public static List<Item> GetShopItem()
        {
            List<Item> item = new List<Item>
            {
                new Item("무쇠갑옷", 0, 5, "무쇠로 만들어져 튼튼한 갑옷입니다.", 1000, ItemType.Armor),
                new Item("스파르타의 창", 7, 0, "스파르타의 전사들이 사용했다는 전설의 창입니다.", 2500, ItemType.Weapon),
                new Item("낡은 검", 2, 0, "쉽게 볼 수 있는 낡은 검입니다.", 600, ItemType.Weapon),
                new Item("반짝이는 총", 5, 0, "그냥 반짝거리는 총입니다...", 1700, ItemType.Weapon)
            };

            return item;
        }

        public static void SellItem(Player player)
        {
            SetColor(ConsoleColor.Blue);
            Console.WriteLine("상점 - 아이템 판매");
            Console.WriteLine("필요하지 않은 아이템을 판매 할 수 있습니다. ");
            ResetColor();
            Console.WriteLine();
            Console.Write("[보유 골드] ");
            SetColor(ConsoleColor.Green);
            Console.Write($"{player.gold}");
            ResetColor();
            Console.WriteLine(" G");
            Console.WriteLine();

            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < player.inventory.Count; i++)
            {
                Item item = player.inventory[i];

                double price = item.Price * 0.85;
                string equippedStatus = item.Equip ? "[E]" : "";
                Console.WriteLine($"{i + 1}. {equippedStatus} {item.Name} | {item.Explain} | {price:F0} G ");
            }

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요");
            SetColor(ConsoleColor.Yellow);
            Console.Write(">>");
            ResetColor();

            string input = Console.ReadLine();

            if (input == "0")
            {
                store(player);
                return;
            }

            int itemIndex; 

            if (int.TryParse(input, out itemIndex) && itemIndex >= 1 && itemIndex <= player.inventory.Count)
            {
                Item selectItem = player.inventory[itemIndex - 1];

                // 아이템이 장착되어 있으면 장착 해제
                if (selectItem.Equip)
                {
                    player.attack -= selectItem.Attack;
                    player.shield -= selectItem.Shield;
                    selectItem.Equip = false; // 장착 해제
                }

                // 아이템 판매 처리
                double sell = selectItem.Price * 0.85; // 85% 가격
                player.gold += (int)sell; // 골드 추가
                player.inventory.Remove(selectItem); // 아이템 삭제
                //player.inventory.Count -= -1;

                Console.WriteLine($"{selectItem.Name} 아이템을 {sell:F0}G에 판매했습니다.");
                Thread.Sleep(2000); // 2초 대기
                Console.Clear();
                store(player); // 상점으로 돌아가기
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Thread.Sleep(1000); // 1초 대기
                SellItem(player); // 다시 아이템 판매 메뉴로 돌아가기
            }
        }
          
        public static void BuyItem(Player player)
        {
            List<Item> shopItem = GetShopItem();

            SetColor(ConsoleColor.Blue);
            Console.WriteLine("상점 - 아이템 구매");
            Console.WriteLine("필요한 아이템을 얻을 수 있습니다.");
            ResetColor();
            Console.WriteLine();
            Console.Write("[보유 골드] ");
            SetColor(ConsoleColor.Green);
            Console.Write($"{player.gold}");
            ResetColor();
            Console.WriteLine(" G");
            Console.WriteLine();


            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < shopItem.Count; i++)
            {
                Item item = shopItem[i];

                //구매 완료된 아이템 표시
                string status = player.inventory.Any(i => i.Name == item.Name) ? "[구매완료]" : item.Price.ToString() + "G"; ;
                Console.WriteLine($"{i + 1}. {item.Name} | {item.Type} | {item.Explain} | {status} ");
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요");
            Console.Write(">>");

            string input = Console.ReadLine();

            if (input == "0")
            {
                store(player);
                return;
            }

            int itemIndex;
            
            if (int.TryParse(input, out itemIndex) && itemIndex >= 1 && itemIndex <= shopItem.Count)
            {
                Item selectItem = shopItem[itemIndex - 1];

                if (player.inventory.Any(i => i.Name == selectItem.Name))
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                    Thread.Sleep(1000); // 1초 대기
                    BuyItem(player); // 다시 아이템 구매 메뉴로 돌아가기
                }
                else
                {
                    if (player.gold >= selectItem.Price)
                    {
                        player.gold -= selectItem.Price;
                        player.inventory.Add(selectItem); // 인벤토리에 아이템 추가
                        Console.WriteLine($"{selectItem.Name} 아이템을 구매했습니다. 3초뒤 상점으로 돌아갑니다.");
                        Thread.Sleep(3000); //3초 대기
                        store(player); // 구매 후 바로 상점 화면으로 돌아가기
                    }
                    else
                    {
                        Console.WriteLine("Gold가 부족합니다.");
                        Thread.Sleep(1000); // 1초 대기
                        BuyItem(player); // 골드 부족시 다시 아이템 구매 메뉴로 돌아가기
                    }
                }
                
                
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Thread.Sleep(1000); // 1초 대기
                BuyItem(player); // 잘못된 입력시 다시 아이템 구매 메뉴로 돌아가기
            }
        }
        
        public static void store(Player player)
        {
            List<Item> shopItem = GetShopItem();

            Console.Clear();
            SetColor(ConsoleColor.Blue);
            Console.WriteLine("상점");
            Console.WriteLine("필요한 아이템을 얻을 수 있습니다.");
            ResetColor();
            Console.WriteLine();
            Console.Write("[보유 골드] ");
            SetColor(ConsoleColor.Green);
            Console.Write($"{player.gold}");
            ResetColor();
            Console.WriteLine(" G");
            Console.WriteLine();

            Console.WriteLine("[아이템 목록]");
            
            for (int i = 0; i < shopItem.Count; i++)
            {
                Item item = shopItem[i];
                string status = player.inventory.Any(i => i.Name == item.Name && i.Price == item.Price) ? "구매완료" : item.Price.ToString() + "G";
                Console.WriteLine($"- {item.Name} | {item.Explain} | {status} "); 
            }
            Console.WriteLine();
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요");
            Console.Write(">>");

            string input = Console.ReadLine();

            if (input == "0")
            {
                start(player);

            }
            else if (input == "1")
            {
                Console.Clear();
                BuyItem(player);
            }
            else if (input == "2")
            {
                Console.Clear();
                SellItem(player);
            }
            else
            {
                Console.WriteLine("잘못된 번호 입니다.");
                Thread.Sleep(1000); // 1초 대기
                store(player); // 잘못된 입력 시 다시 상점 메뉴로 돌아가기
            }
        }
        public static void EquipItem(Player player)
        {
            
            Console.Clear();
            SetColor(ConsoleColor.Blue);
            Console.WriteLine("인벤토리 - 장착 관리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            ResetColor();
            Console.WriteLine();
            SetColor(ConsoleColor.Yellow);
            Console.WriteLine("[아이템 목록]");
            ResetColor();
            
            if (player.inventory.Count == 0)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("보유한 아이템이 없습니다. 2초 뒤 인벤토리로 돌아갑니다.");
                ResetColor();
                Thread.Sleep(2000);
                InventoryView(player);
            }
            else
            {
                for (int i = 0; i < player.inventory.Count; i++)
                {
                    Item item = player.inventory[i];

                    string equippedStatus = item.Equip ? "[E]" : "";
                    Console.WriteLine($"{i + 1}. {equippedStatus} {item.Name} | {item.Explain}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요");
            SetColor(ConsoleColor.Yellow);
            Console.Write(">>");
            ResetColor();
            string input = Console.ReadLine();

            if (input == "0")
            {
                start(player);
                return;
            }

            int itemIndex;

            if (int.TryParse(input, out itemIndex) && itemIndex >= 1 && itemIndex <= player.inventory.Count)
            {
                Item selectItem = player.inventory[itemIndex - 1];

                if (selectItem.Type == ItemType.Armor) // 방어구 장착
                {
                    //기존 방어구 있으면 해제
                    Item currentArmor = player.inventory.FirstOrDefault(i => i.Type == ItemType.Armor && i.Equip);
                    if (currentArmor != null)
                    {
                        currentArmor.Equip = false;
                        player.shield -= currentArmor.Shield; // 기존 방어구 해제
                    }

                    player.shield += selectItem.Shield;

                }
                else if (selectItem.Type == ItemType.Weapon) // 무기 장착
                {
                    // 기존 무기가 있으면 해제
                    Item currentWeapon = player.inventory.FirstOrDefault(i => i.Type == ItemType.Weapon && i.Equip);
                    if (currentWeapon != null)
                    {
                        currentWeapon.Equip = false;
                        player.attack -= currentWeapon.Attack; // 기존 무기 해제
                    }

                    // 새 무기 장착
                    player.attack += selectItem.Attack;
                }

                selectItem.Equip = !selectItem.Equip; // 아이템 장착/해제 
                Console.WriteLine(selectItem.Equip ? $"{selectItem.Name}을(를) 장착했습니다." : $"{selectItem.Name}을(를) 장착 해제했습니다.");
                Thread.Sleep(1000);
                Console.Clear();
                EquipItem(player);
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Thread.Sleep(1000);
                EquipItem(player); 
            }

        }
        public static void InventoryView(Player player)
        {
            Console.Clear();
            SetColor(ConsoleColor.Blue);
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            ResetColor();
            Console.WriteLine();
            SetColor(ConsoleColor.Yellow);
            Console.WriteLine("[아이템 목록]");
            ResetColor();

            for (int i = 0; i < player.inventory.Count; i++)
            {
                Item item = player.inventory[i];

                string equippedStatus = item.Equip ? "[E]" : "";
                Console.WriteLine($"- {equippedStatus} {item.Name} | {item.Explain}");
            }
            Console.WriteLine();
            Console.WriteLine("1. 장착 관리");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요");
            SetColor(ConsoleColor.Yellow);
            Console.Write(">>");
            ResetColor();

            string input = Console.ReadLine();

            if (input == "0")
            {
                start(player);

            }
            else if (input == "1")
            {
                EquipItem(player);
            }
            else
            {
                Console.WriteLine("잘못된 번호 입니다.");
                Thread.Sleep(1000);
                InventoryView(player);
            }
        }
        public static void PlayerInfoView(Player player)
        {

            Console.WriteLine($"Lv . {player.level}" );
            Console.Write("닉네임 : ");
            SetColor(ConsoleColor.DarkMagenta);
            Console.WriteLine($"{player.name}");
            ResetColor();
            Console.WriteLine($"직업 : " + player.chad);
            Console.WriteLine($"공격력 : " + player.attack);
            Console.WriteLine($"방어력 : " + player.shield);
            Console.WriteLine($"체력 : " + player.hp);
            Console.WriteLine($"Gold : " + player.gold);
            
        }
        public static void info(Player player)
        {
            Console.Clear();
            SetColor(ConsoleColor.Blue);
            Console.WriteLine("상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            ResetColor();
            Console.WriteLine();

            PlayerInfoView(player);
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요");
            SetColor(ConsoleColor.Yellow);
            Console.Write(">>");
            ResetColor();
            string input = Console.ReadLine();

            if (input == "0")
            {
                start(player);
            }
            else
            {
                Console.WriteLine("잘못된 번호 입니다.");
                Thread.Sleep(1000);
                info(player);
            }
        }

        public static void start(Player player)
        {
            Console.Clear();
            SetColor(ConsoleColor.Blue);
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            ResetColor();
            Console.WriteLine();
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전입장");
            Console.WriteLine("5. 휴식하기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            SetColor(ConsoleColor.Yellow);
            Console.Write(">> ");
            ResetColor();

            string Start = Console.ReadLine();

            if (Start == "1")
            {
                info(player);
            }
            else if (Start == "2")
            {
                InventoryView(player);
            }
            else if (Start == "3")
            {
                store(player);
            }
            else if (Start == "4")
            {
                Console.Clear();
                Dungeon(player);
            }
            else if (Start == "5")
            {
                Console.Clear();
                Rest(player);
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다. 1초 뒤 다시 입력해주세요");
                Thread.Sleep(1000);
                Console.Clear();
                start(player);
            }
        }

        public static void NicknameSetting(Player player)
        {
            SetColor(ConsoleColor.Yellow);
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("◎ 스파르타 마을 ◎");
            ResetColor();
            Console.WriteLine();
            Console.Write("사용할 플레이어 닉네임을 입력해주세요 : ");
            player.name = Console.ReadLine();

            InitPlayerInfo(player);

            Console.WriteLine();
            Console.Write("입력하신 닉네임은 ");//{player.name} 입니다.");
            SetColor(ConsoleColor.Magenta);
            Console.Write($"{player.name}");
            ResetColor();
            Console.WriteLine(" 입니다.");
            Console.WriteLine();
            Console.WriteLine("1. 이거로 할래요!");
            Console.WriteLine("2. 취소");
            Console.WriteLine();
            Console.WriteLine("원하시는 번호를 입력해주세요 : ");
            SetColor(ConsoleColor.Yellow);
            Console.Write(">> ");
            ResetColor();

            string input = Console.ReadLine();

            int num = int.Parse(input);

            if (num == 1)
            {
                Console.Clear();
                start(player);
            }
            else if (num == 2)
            {
                Console.Clear();
                NicknameSetting(player);

            }
            else
            {
                Console.WriteLine("잘못 된 번호 입니다. 1초 뒤 다시 입력해주세요");
                Thread.Sleep(1000);
                Console.Clear();
                NicknameSetting(player);
            }
        }
        public static void InitPlayerInfo(Player player)
        {
            player.level = 1;
            player.chad = "아직 직업 없음";
            player.attack = 10;
            player.shield = 5;
            player.hp = 100;
            player.gold = 10000;
            player.clearcount = 0;
        }

        
        public static void Main(string[] args)
        {
            Player player = new Player();

            //캐릭터 닉네임 생성
            NicknameSetting(player);
        }
    }
}
