import java.util.Scanner;

public class Program {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        Employee[] staff = new Employee[5];

        // Создание объектов
        staff[0] = new Worker("Иван", 30, "Производство", "Ночная");
        staff[1] = new Engineer("Мария", 35, "Проектирование", "Механика");
        staff[2] = new Administrator("Ольга", 40, "Управление", 2);
        staff[3] = new Worker("Петр", 28, "Сборка", "Дневная");
        staff[4] = new Engineer("Анна", 32, "Разработка", "Электроника");

        // Меню
        while (true) {
            System.out.println("\nМеню:");
            System.out.println("1. Показать всех сотрудников");
            System.out.println("2. Проверить equals");
            System.out.println("3. Показать количество сотрудников");
            System.out.println("4. Вызвать метод work()");
            System.out.println("0. Выход");
            System.out.print("Выберите пункт: ");

            if (!scanner.hasNextInt()) { // защита от ввода не-чисел
                System.out.println("Введите число!");
                scanner.next();
                continue;
            }

            int choice = scanner.nextInt();

            switch (choice) {
                case 1 -> {
                    for (Employee e : staff) {
                        System.out.println(e.toString());
                    }
                }
                case 2 -> System.out.println("Сравнение staff[0] и staff[3]: " + staff[0].equals(staff[3]));
                case 3 -> System.out.println("Всего сотрудников: " + Employee.getCount());
                case 4 -> {
                    for (Employee e : staff) {
                        e.work(); // полиморфизм
                    }
                }
                case 0 -> {
                    System.out.println("Выход...");
                    return;
                }
                default -> System.out.println("Неверный выбор.");
            }
        }
    }
}