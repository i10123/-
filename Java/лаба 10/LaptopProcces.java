import java.io.*;
import java.util.*;

public class LaptopProcces {
    private final String filePath = "ASUSlaptops.dat";

    public void resetFile(List<Laptop> laptops) {
        try (var out = new ObjectOutputStream(new BufferedOutputStream(new FileOutputStream(filePath)))) {
            for (var laptop : laptops) 
                out.writeObject(laptop);
        } catch (IOException e) {
            System.out.println("Ошибка записи: " + e.getMessage());
        }
    }

    public void showAll() {
        try (var in = new ObjectInputStream(new BufferedInputStream(new FileInputStream(filePath)))) {
            while (true) 
                System.out.println((Laptop) in.readObject());
        } catch (EOFException e) { 
            System.out.println(" Конец списка."); 
        } catch (Exception e) {
            System.out.println("Ошибка чтения: " + e.getMessage());
        }
    }

    public void addLaptop(Laptop laptop) {
        try (var add = new AppendStream(new BufferedOutputStream(new FileOutputStream(filePath, true)))) {
            add.writeObject(laptop);
            System.out.println("Ноутбук добавлен.");
        } catch (IOException e) {
            System.out.println("Ошибка добавления: " + e.getMessage());
        }
    }

    public void showAsus() {
        double total = 0;
        try (var in = new ObjectInputStream(new BufferedInputStream(new FileInputStream(filePath)))) {
            System.out.println("Asus с CPU > 2.0 GHz:");
            while (true) {
                var l = (Laptop) in.readObject();
                if (l.getName().toLowerCase().contains("asus") && l.getCpuGHz() > 2.0) {
                    System.out.println(l);
                    total += l.getPrice();
                }
            }
        } catch (EOFException e) {
            System.out.printf("Общая стоимость: $%.2f%n", total);
        } catch (Exception e) {
            System.out.println("Ошибка фильтрации: " + e.getMessage());
        }
    }
}