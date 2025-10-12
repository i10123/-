import java.util.HashMap;
import java.util.Map;
import java.util.Scanner;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class lab8 {
    static String text = """
        Mama idet domoy s synom i neset sumku.
        Papa pishet /knigu i smotrit/ v okno doma.
        Na stole lezhit karta i staraya kniga.
        Deti igrayut vo dvore ryadom s zelenim domikom.
        V lesy rastut griby a tuman ochen silen.
        Babka gotovit sup iz luka morkovki i kartoshki.
        V komnate stoit stul y okna bolshoy stol.
        Ona smotrit a solnce ulybaetsya veselo.
        Uchenik /pishet v tetrad/ i slushaet uroki.
        Mashina edet po doroge mezhdu lesami.
        On lyubit pesni poetomu igraet na gitare doma.
        Vesnoy ptitsy letayut i stroyat gnezda.
        V parke deti kormyat ptits i begayut veselo.
        V kuhne stoyat /lozhki i chashki/ s chaiem.
        Doma chitayem knigi i smotrim kino vmeste.
        """;

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        boolean running = true;

        while (running) {
            System.out.println("Главное меню:");
            System.out.println("1 - Часть 1: Удаление повторяющихся букв");
            System.out.println("2 - Часть 2: Удаление текста между символами");
            System.out.println("0 - Выход из программы");
            System.out.print("Выберите действие: ");
            String choice = scanner.nextLine();

            switch (choice) {
                case "1" -> part1(scanner);
                case "2" -> part2(scanner);
                case "0" -> {
                    System.out.println("Выход из программы.");
                    running = false;
                }
                default -> System.out.println("Неверный выбор. Попробуйте снова.");
            }
        }
    }

    // === ЧАСТЬ 1 ===
    private static void part1(Scanner scanner) {
        String[] lines = text.split("\n"); // Разделение по строкам
        while (true) {
            printAllLines(lines);
            System.out.print("Введите номер строки для обработки (или '0' для возврата): ");
            String input = scanner.nextLine();

            int index;

            try {
                index = Integer.parseInt(input);
            } catch (NumberFormatException e) {
                System.out.println("Ошибка: введите целое число.");
                continue;
            }

            if (index == 0) 
                break;

            if (index < 1 || index > lines.length) {
                System.out.println("Нет такой строки. Попробуйте снова.");
                continue;
            }

            String line = lines[index - 1];
            System.out.println("\n---------------------------------------------");
            System.out.println("Исходная строка: " + line);

            Pattern wordPattern = Pattern.compile("\\b[a-zA-Z]{2,10}\\b");
            Matcher matcher = wordPattern.matcher(line);

            String lastWord = null;
            while (matcher.find()) {
                lastWord = matcher.group();
            }

            matcher.reset();

            System.out.print("Результат: ");
            while (matcher.find()) {
                String word = matcher.group();
                if (!word.equals(lastWord)) {
                    System.out.print(deleteRepet(word) + " ");
                }
            }
            System.out.println("\n---------------------------------------------");
        }
    }

    // === ЧАСТЬ 2 ===
    private static void part2(Scanner scanner) {
        System.out.print("Введите начальный символ: ");
        String start = scanner.nextLine();
        System.out.print("Введите конечный символ: ");
        String end = scanner.nextLine();

        String deleted = Pattern.quote(start) + ".*?" + Pattern.quote(end);
        String cleaned = text.replaceAll(deleted, "");

        System.out.println("\n---------------------------------------------");
        System.out.println("Результат:");
        System.out.print(cleaned);
        System.out.println("---------------------------------------------");
    }

    private static String deleteRepet(String word) {
        StringBuilder result = new StringBuilder();
        Map<Character, Integer> seen = new HashMap<>();

        for (char c : word.toCharArray()) {
            char lower = Character.toLowerCase(c);
            if (!seen.containsKey(lower)) {
                result.append(c);
                seen.put(lower, 1);
            }
        }
        return result.toString();
    }

    private static void printAllLines(String[] lines) {
        System.out.println("\nСписок строк:");
        for (int i = 0; i < lines.length; i++) {
            System.out.printf("%2d: %s%n", i + 1, lines[i]);
        }
    }
}