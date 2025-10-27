public class Cpu {
    private CpuProcess current_process = null;

    public boolean isBusy() { 
        return current_process != null; 
    }

    public void start(CpuProcess process, double currentTime) {
        current_process = process;
        process.setStartServiceTime(currentTime);
        process.setEndServiceTime(currentTime + process.getServiceTime());
    }

    public CpuProcess finish() {
        CpuProcess process = current_process;
        current_process = null;
        return process;
    }

    public CpuProcess getCurrentProcess() { 
        return current_process; 
    }
}