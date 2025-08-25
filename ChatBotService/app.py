from flask import Flask, request, jsonify
import google.generativeai as genai
from langchain_community.document_loaders import PyPDFLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_community.vectorstores import FAISS
from langchain_community.embeddings import HuggingFaceEmbeddings
import warnings
import os

warnings.filterwarnings("ignore", category=FutureWarning)

GOOGLE_API_KEY = "AIzaSyA2LcsAf0pks-KWnhKzkfAeep5-bwm2AZY"
genai.configure(api_key=GOOGLE_API_KEY)

app = Flask(__name__)

vectorstore = None
retriever = None

def process_pdf(file_path):
    global vectorstore, retriever
    loader = PyPDFLoader(file_path)
    documents = loader.load()
    text_splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=50)
    docs = text_splitter.split_documents(documents)
    embeddings = HuggingFaceEmbeddings(model_name="all-MiniLM-L6-v2")
    vectorstore = FAISS.from_documents(docs, embeddings)
    retriever = vectorstore.as_retriever()

@app.route("/upload", methods=["POST"])
def upload():
    if "file" not in request.files:
        return jsonify({"error": "No file provided"}), 400

    pdf_file = request.files["file"]
    file_path = os.path.join("uploads", pdf_file.filename)
    os.makedirs("uploads", exist_ok=True)
    pdf_file.save(file_path)

    process_pdf(file_path)
    return jsonify({"message": "PDF processed successfully"})

@app.route("/ask", methods=["POST"])
def ask():
    global retriever
    if retriever is None:
        return jsonify({"error": "No PDF uploaded yet"}), 400

    user_query = request.json.get("query", "")
    docs = retriever.invoke(user_query)
    context = "\n\n".join([doc.page_content for doc in docs])

    model = genai.GenerativeModel(model_name='gemini-1.5-flash')
    prompt = f"You are a helpful assistant.\n\n{context}\n\nQuestion: {user_query}"
    response = model.generate_content(prompt)
    return jsonify({"answer": response.text})

@app.route("/")
def index():
    return "Server is running. Try /upload or /chat."


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
