import java.util.*;

public class Main {
    public static void main(String[] args) {
        List<Student> students = new ArrayList<>(List.of(
            new Student("Иванов Иван Сергеевич", "Гродно", "ул. Советская", "10", "5", 278),
            new Student("Петров Пётр Андреевич", "Витебск", "пр-т Ленина", "3", "2", 290),
            new Student("Сидоров Алексей Николаевич", "Гомель", "ул. Мира", "7", "1", 310),
            new Student("Козлов Андрей Дмитриевич", "Брест", "ул. Победы", "9", "4", 345),
            new Student("Смирнова Анна Викторовна", "Могилёв", "ул. Парковая", "5", "3", 200),
            new Student("Фёдоров Сергей Александрович", "Орша", "ул. Центральная", "12", "1", 220),
            new Student("Кравченко Ольга Павловна", "Бобруйск", "ул. Октябрьская", "15", "2", 140),
            new Student("Романов Игорь Евгеньевич", "Полоцк", "ул. Молодёжная", "8", "1", 215),
            new Student("Тимошенко Дарья Владимировна", "Речица", "ул. Пушкина", "14", "1", 250),
            new Student("Ковалёв Денис Олегович", "Лида", "ул. Гагарина", "6", "2", 170),
            new Student("Гончарова Мария Сергеевна", "Новогрудок", "ул. Кирова", "9", "1", 150),
            new Student("Мельник Елена Васильевна", "Жлобин", "ул. Зелёная", "2", "3", 250),
            new Student("Андреев Роман Николаевич", "Слуцк", "ул. Фрунзе", "3", "1", 110),
            new Student("Данилова Виктория Алексеевна", "Мозырь", "ул. Набережная", "5", "5", 270),
            new Student("Климов Николай Игоревич", "Борисов", "ул. Спортивная", "11", "3", 80)
        ));

        Scanner scanner = new Scanner(System.in);

        while (true) {
            System.out.println("\n1. Показать всех студентов");
            System.out.println("2. Добавить студента");
            System.out.println("3. Удалить студента");
            System.out.println("4. Показать очередь в общежитие");
            System.out.println("5. Выход");
            System.out.print("Выберите пункт: ");

            int choice;
            try {
                choice = Integer.parseInt(scanner.nextLine());
            } catch (NumberFormatException e) {
                System.out.println("Введите число!");
                continue;
            }

            switch (choice) {
                case 1 -> showAll(students);
                case 2 -> addStudent(students, scanner);
                case 3 -> removeStudent(students, scanner);
                case 4 -> showQueue(students, scanner);
                case 5 -> {
                    System.out.println("Выход из программы...");
                    return;
                }
                default -> System.out.println("Неверный выбор!");
            }
        }
    }

    private static void showAll(List<Student> students) {
        if (students.isEmpty()) {
            System.out.println("Коллекция пуста!");
            return;
        }
        for (int i = 0; i < students.size(); i++) {
            System.out.println((i + 1) + ". " + students.get(i));
        }
    }

    private static void addStudent(List<Student> students, Scanner scanner) {
        System.out.print("Введите ФИО: ");
        String name = scanner.nextLine();
        System.out.print("Введите город: ");
        String city = scanner.nextLine();
        System.out.print("Введите улицу: ");
        String street = scanner.nextLine();
        System.out.print("Введите номер дома: ");
        String house = scanner.nextLine();
        System.out.print("Введите номер квартиры: ");
        String apartment = scanner.nextLine();
        System.out.print("Введите расстояние до Минска: ");
        double distance;
        try {
            distance = Double.parseDouble(scanner.nextLine());
        } catch (NumberFormatException e) {
            System.out.println("Некорректное расстояние!");
            return;
        }

        students.add(new Student(name, city, street, house, apartment, distance));
        System.out.println("Студент добавлен!");
    }

    private static void removeStudent(List<Student> students, Scanner scanner) {
        showAll(students);
        System.out.print("Введите номер для удаления: ");
        int index;
        try {
            index = Integer.parseInt(scanner.nextLine()) - 1;
        } catch (NumberFormatException e) {
            System.out.println("Введите число!");
            return;
        }

        if (index >= 0 && index < students.size()) {
            students.remove(index);
            System.out.println("Студент удалён!");
        } else {
            System.out.println("Неверный номер!");
        }
    }

    private static void showQueue(List<Student> students, Scanner scanner) {
        System.out.print("Введите количество мест (k): ");
        int k;
        try {
            k = Integer.parseInt(scanner.nextLine());
        } catch (NumberFormatException e) {
            System.out.println("Введите число!");
            return;
        }

        List<Student> queue = students.stream().sorted().limit(k).toList();

        System.out.println("\nСтуденты, заселяемые в первую очередь:");
        for (int i = 0; i < queue.size(); i++) {
            System.out.println((i + 1) + ". " + queue.get(i));
        }
    }
}