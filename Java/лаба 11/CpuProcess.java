public class CpuProcess {
    private final int id;
    private final double birthTime;
    private final double serviceTime;
    private double startServiceTime = -1;
    private double endServiceTime = -1;

    public CpuProcess(int id, double birthTime, double serviceTime) {
        this.id = id;
        this.birthTime = birthTime;
        this.serviceTime = serviceTime;
    }

    public int getId() { 
        return id; 
    }
    public double getBirthTime() { 
        return birthTime; 
    }
    public double getServiceTime() { 
        return serviceTime; 
    }
    public double getStartServiceTime() { 
        return startServiceTime; 
    }
    public double getEndServiceTime() { 
        return endServiceTime; 
    }

    public void setStartServiceTime(double time) { 
        this.startServiceTime = time; 
    }
    public void setEndServiceTime(double time) { 
        this.endServiceTime = time; 
    }

    public double getWaitingTime() {
        if (startServiceTime < 0) 
            return -1;
        return startServiceTime - birthTime;
    }

    public double getTotalTimeInSystem() {
        if (endServiceTime < 0) 
            return -1;
        return endServiceTime - birthTime;
    }

    @Override
    public String toString() {
        return "id=" + id;
    }
}