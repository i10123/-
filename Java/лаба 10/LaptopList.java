import java.util.*;

public class LaptopList {
    public static List<Laptop> getLaptopList() {
        List<Laptop> list = new ArrayList<>();
        list.add(new Laptop("Asus Zen", 2.5, 8, 512, 850));
        list.add(new Laptop("Asus Pro", 3.0, 16, 1024, 1200));
        list.add(new Laptop("Xiaomi Air", 2.2, 8, 256, 700));
        list.add(new Laptop("Xiaomi Book", 1.8, 4, 128, 450));
        list.add(new Laptop("Lenovo Think", 2.4, 16, 512, 950));
        list.add(new Laptop("Lenovo Idea", 2.0, 8, 256, 600));
        list.add(new Laptop("Apple Mac", 3.2, 16, 1024, 2000));
        list.add(new Laptop("Apple Air", 2.1, 8, 512, 1300));
        list.add(new Laptop("Gigabyte Aero", 2.6, 16, 1024, 1400));
        list.add(new Laptop("Gigabyte G5", 2.3, 8, 512, 900));
        list.add(new Laptop("Huawei Mate", 2.0, 8, 256, 650));
        list.add(new Laptop("Huawei D14", 1.9, 4, 128, 400));
        list.add(new Laptop("Asus Vivo", 2.1, 8, 512, 800));
        list.add(new Laptop("Asus TUF", 3.1, 16, 1024, 1100));
        list.add(new Laptop("Xiaomi Pro", 2.5, 16, 512, 950));
        list.add(new Laptop("Lenovo Yoga", 2.3, 8, 256, 750));
        list.add(new Laptop("Apple Pro", 3.0, 32, 2048, 2500));
        list.add(new Laptop("Gigabyte U4", 2.2, 8, 512, 850));
        list.add(new Laptop("Huawei X", 2.4, 16, 512, 1000));
        list.add(new Laptop("Asus ROG", 3.5, 32, 2048, 2800));
        return list;
    }
}