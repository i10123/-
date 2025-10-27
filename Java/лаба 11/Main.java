import java.util.*;

public class Main {
    private static final int COUNT_PROCESSES = 2000;
    private static final double GENERATION_PROCESS_LOW = 0.1;
    private static final double GENERATION_PROCESS_HIGH = 0.4;
    private static final double SERVICE_TIME_LOW = 0.3;
    private static final double SERVICE_TIME_HIGH = 0.6;

    public static void main(String[] args) {
        try (Scanner scanner = new Scanner(System.in)) {
            System.out.print("Выберите формат логов (1=подробный, 2=краткий): ");
            int logFormat = Integer.parseInt(scanner.nextLine().trim());

            Simulator simulator = new Simulator(COUNT_PROCESSES, GENERATION_PROCESS_LOW, GENERATION_PROCESS_HIGH, SERVICE_TIME_LOW, SERVICE_TIME_HIGH, logFormat);
            simulator.run();
        }
    }
}