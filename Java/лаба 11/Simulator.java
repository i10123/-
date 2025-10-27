import java.util.*;

public class Simulator {
    private final int totalProcesses;
    private final double generationProcessLow, generationProcessHigh, serviceTimeLow, serviceTimeHigh;
    private final boolean detailedLogs;

    private final CpuQueue queue = new CpuQueue();  // ожидающие процессы
    private final Cpu cpu = new Cpu();              // процессор и текущий обслуживаемый процесс

    private double time = 0.0;  // текущее время
    private int generated = 0;  // счётчик сгенерированных процессов.
    private int served = 0;     // счётчик обслуженных процессов.
    private double nextGenerationTime = Double.POSITIVE_INFINITY;   // момент следующего события генерации
    private double nextFinishTime = Double.POSITIVE_INFINITY;       // момент окончания обслуживания
    private double cpuBusyTime = 0.0;

    private double sumWaiting = 0.0; // сумма ожиданий
    private double maxWaiting = 0.0; // максимум времени ожидания

    public Simulator(int totalProcesses, double generationProcessLow, double generationProcessHigh, double serviceTimeLow, double serviceTimeHigh, int logFormat) {
        this.totalProcesses = totalProcesses;
        this.generationProcessLow = generationProcessLow;
        this.generationProcessHigh = generationProcessHigh;
        this.serviceTimeLow = serviceTimeLow;
        this.serviceTimeHigh = serviceTimeHigh;
        this.detailedLogs = (logFormat == 1);
        if (totalProcesses > 0) 
            timeNextGeneration(0.0);
    }

    private double randBetween(double low, double high) {
        return low + (high - low) * Math.random();
    }

    private void timeNextGeneration(double baseTime) {
        if (generated >= totalProcesses) {
            nextGenerationTime = Double.POSITIVE_INFINITY;
            return;
        }
        double interval = randBetween(generationProcessLow, generationProcessHigh);
        nextGenerationTime = baseTime + interval;
    }

    private void timeFinishForCurrentProcess() {
        CpuProcess process = cpu.getCurrentProcess();
        if (process == null)
            nextFinishTime = Double.POSITIVE_INFINITY;
        else 
            nextFinishTime = process.getEndServiceTime();
    }

    private void log(String s) {
        System.out.println(s);
    }

    private String timeStr(double t) {
        return String.format(Locale.US, "%.3f", t);
    }

    public void run() {
        timeFinishForCurrentProcess();

        while (generated < totalProcesses || cpu.isBusy() || queue.size() > 0) {
            double nextEventTime = Math.min(nextGenerationTime, nextFinishTime);
            if (cpu.isBusy()) 
                cpuBusyTime += (nextEventTime - time);
            time = nextEventTime;

            if (detailedLogs) 
                log(String.format("Время=%s -> Состояние перед событием: очередь=%d | процессор %s",
                timeStr(time), queue.size(), cpu.isBusy() ? "Работает" : "Отдыхает"));

            // Обработка события генерации
            if (Math.abs(time - nextGenerationTime) < 1e-3) {
                generated++;
                double serviceTime = randBetween(serviceTimeLow, serviceTimeHigh);
                CpuProcess process = new CpuProcess(generated, time, serviceTime);
                if (cpu.isBusy()) {
                    queue.addToQueue(process);
                    if (detailedLogs) 
                        log(String.format("Время=%s -> Событие:генерация | %s помещено в очередь | очередь=%d",
                        timeStr(time), process, queue.size()));
                } else {
                    cpu.start(process, time);
                    timeFinishForCurrentProcess();
                    if (detailedLogs) 
                        log(String.format("Время=%s -> Событие=генерация | %s обслуживается | service=%.3f | ожидаемое окончание=%s",
                        timeStr(time), process, process.getServiceTime(), timeStr(process.getEndServiceTime())));
                }
                timeNextGeneration(time);
            }

            // Обработка события окончания обслуживания
            if (Math.abs(time - nextFinishTime) < 1e-3) {
                CpuProcess finished = cpu.finish();
                finished.setEndServiceTime(time);
                served++;
                double wait = finished.getWaitingTime();
                if (wait >= 0) {
                    sumWaiting += wait;
                    if (wait > maxWaiting) 
                        maxWaiting = wait;
                }
                if (detailedLogs) 
                    log(String.format("Время=%s -> Событие:окончание | %s | ожидание=%.3f | полное время=%.3f",
                    timeStr(time), finished, finished.getWaitingTime(), finished.getTotalTimeInSystem()));
                CpuProcess next = queue.dequeue();
                if (next != null) {
                    cpu.start(next, time);
                    if (detailedLogs) 
                        log(String.format("Время=%s -> Событие:взято из очереди | %s | очередь=%d | начато обслуживание до=%s",
                        timeStr(time), next, queue.size(), timeStr(next.getEndServiceTime())));
                }
                timeFinishForCurrentProcess();
            }
        }

        // Итоги
        double avgWaiting = served > 0 ? sumWaiting / served : 0.0;
        double totalSimTime = time;
        double cpuUtil = totalSimTime > 0 ? (cpuBusyTime / totalSimTime) * 100.0 : 0.0;

        log("");
        log(String.format("Сгенерировано: %d, Обслужено: %d", generated, served));
        log(String.format("Максимальная длина очереди: %d", queue.getMaxLength()));
        log(String.format("Среднее время ожидания: %.3f сек, Максимальное ожидание: %.3f сек", avgWaiting, maxWaiting));
        log(String.format("Загрузка CPU: %.2f%% (занято %.3f / %.3f сек)", cpuUtil, cpuBusyTime, totalSimTime));
    }
}