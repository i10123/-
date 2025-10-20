import java.io.*;

public class AppendStream extends ObjectOutputStream {
    public AppendStream(OutputStream out) throws IOException {
        super(out);
    }

    @Override
    protected void writeStreamHeader() {
    }
}