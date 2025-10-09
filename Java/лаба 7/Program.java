import employees.*;
import java.util.Scanner;

public class Program {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        Employee[] employee = new Employee[10];
        int count = 0;

        // сотрудники
        employee[count++] = new Worker("Иван", 30, "Производство", "Ночная");
        employee[count++] = new Engineer("Мария", 35, "Проектирование", "Механика");
        employee[count++] = new Administrator("Ольга", 40, "Управление", 2);
        employee[count++] = new HR("Наталья", 38, "Кадры", 12);

        while (true) {
            System.out.println("\n Меню:");
            System.out.println("1. Показать всех сотрудников");
            System.out.println("2. Добавить сотрудника");
            System.out.println("3. Удалить сотрудника");
            System.out.println("4. Сравнить сотрудников");
            System.out.println("5. Вызвать work() для одного");
            System.out.println("6. Вызвать work() для всех");
            System.out.println("7. Показать количество сотрудников");
            System.out.println("0. Выход");
            System.out.print("Выбор: ");

            if (!scanner.hasNextInt()) {
                System.out.println("Введите число.");
                scanner.next();
                continue;
            }

            int choice = scanner.nextInt();
            scanner.nextLine();

            switch (choice) {
                case 1 -> {
                    for (int i = 0; i < count; i++) {
                        System.out.println((i + 1) + ": " + employee[i]);
                    }
                }
                case 2 -> {
                    if (count >= employee.length) {
                        System.out.println("Массив сотрудников заполнен.");
                        break;
                    }
                    System.out.print("Тип (worker/engineer/admin/hr): ");
                    String type = scanner.nextLine().trim().toLowerCase();

                    System.out.print("Имя: ");
                    String name = scanner.nextLine();
                    System.out.print("Возраст: ");
                    int age = scanner.nextInt();
                    scanner.nextLine();
                    System.out.print("Отдел: ");
                    String workArea = scanner.nextLine();

                    switch (type) {
                        case "worker" -> {
                            System.out.print("Смена: ");
                            String workPeriod = scanner.nextLine();
                            employee[count++] = new Worker(name, age, workArea, workPeriod);
                        }
                        case "engineer" -> {
                            System.out.print("Специализация: ");
                            String specialization = scanner.nextLine();
                            employee[count++] = new Engineer(name, age, workArea, specialization);
                        }
                        case "admin" -> {
                            System.out.print("Уровень управления: ");
                            int level = scanner.nextInt();
                            scanner.nextLine();
                            employee[count++] = new Administrator(name, age, workArea, level);
                            System.out.println("Администратор добавлен.");
                        }
                        case "hr" -> {
                            System.out.print("Количество собеседований: ");
                            int meetings = scanner.nextInt();
                            scanner.nextLine();
                            employee[count++] = new HR(name, age, workArea, meetings);
                        }
                        default -> System.out.println("Неизвестный тип.");
                    }
                }
                case 3 -> {
                    System.out.print("Введите номер сотрудника для удаления: ");
                    int number = scanner.nextInt();
                    scanner.nextLine();

                    int index = number - 1;

                    if (index < 0 || index >= count) {
                        System.out.println("Неверный номер.");
                        break;
                    }

                    for (int i = index; i < count - 1; i++) {
                        employee[i] = employee[i + 1];
                    }
                    employee[count--] = null;
                    System.out.println("Сотрудник №" + number + " удалён.");
                }
                case 4 -> {
                    System.out.print("Введите два номера сотрудников для сравнения: ");
                    int n1 = scanner.nextInt();
                    int n2 = scanner.nextInt();
                    scanner.nextLine();

                    int i1 = n1 - 1;
                    int i2 = n2 - 1;

                    if (i1 < 0 || i1 >= count || i2 < 0 || i2 >= count) {
                        System.out.println("Неверные номера.");
                        break;
                    }

                    System.out.println("Результат сравнения: " + employee[i1].equals(employee[i2]));
                }
                case 5 -> {
                    System.out.print("Введите номер сотрудника: ");
                    int number = scanner.nextInt();
                    scanner.nextLine();

                    int index = number - 1;

                    if (index < 0 || index >= count) {
                        System.out.println("Неверный номер.");
                        break;
                    }

                    employee[index].work();
                }
                case 6 -> {
                    for (int i = 0; i < count; i++) {
                        employee[i].work();
                    }
                }
                case 7 -> System.out.println("Всего сотрудников: " + Employee.getCountWorkers());
                case 0 -> {
                    System.out.println("Выход из программы.");
                    return;
                }
                default -> System.out.println("Неверный выбор. Попробуйте снова.");
            }
        }
    }
}