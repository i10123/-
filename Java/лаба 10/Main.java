import java.util.*;

public class Main {
    public static void main(String[] args) {
        LaptopProcces service = new LaptopProcces();
        List<Laptop> Laptops = LaptopList.getLaptopList();
        service.resetFile(Laptops);

        Scanner scanner = new Scanner(System.in);
        while (true) {
            System.out.println("\n Меню:");
            System.out.println("1. Показать все ноутбуки");
            System.out.println("2. Добавить ноутбук");
            System.out.println("3. Показать ноутбуки Asus, где частота процессора > 2GHz");
            System.out.println("4. Перезаписать файл начальными данными");
            System.out.println("0. Выход");
            System.out.print("Ваш выбор: ");
            String choice = scanner.nextLine();

            switch (choice) {
                case "1" -> service.showAll();
                case "2" -> {
                    System.out.print("Название: ");
                    String name = scanner.nextLine();
                    System.out.print("Частота CPU (ГГц): ");
                    double cpu = Double.parseDouble(scanner.nextLine());
                    System.out.print("RAM (ГБ): ");
                    int ram = Integer.parseInt(scanner.nextLine());
                    System.out.print("HDD (ГБ): ");
                    int hdd = Integer.parseInt(scanner.nextLine());
                    System.out.print("Цена ($): ");
                    double price = Double.parseDouble(scanner.nextLine());
                    service.addLaptop(new Laptop(name, cpu, ram, hdd, price));
                }
                case "3" -> service.showAsus();
                case "4" -> {
                    List<Laptop> laptops = LaptopList.getLaptopList();
                    service.resetFile(laptops);
                    System.out.println("Файл перезаписан начальными данными.");
                }
                case "0" -> {
                    System.out.println("Завершение.");
                    return;
                }
                default -> System.out.println("Неверный выбор.");
            }
        }
    }
}