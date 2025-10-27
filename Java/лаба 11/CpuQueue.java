import java.util.*;

public class CpuQueue {
    private final Queue<CpuProcess> queue = new LinkedList<>();
    private int maxLength = 0;

    public void addToQueue(CpuProcess process) {
        queue.add(process);
        if (queue.size() > maxLength) 
            maxLength = queue.size();
    }

    public CpuProcess dequeue() {
        return queue.poll();
    }

    public int size() { 
        return queue.size(); 
    }
    public int getMaxLength() { 
        return maxLength; 
    }
}